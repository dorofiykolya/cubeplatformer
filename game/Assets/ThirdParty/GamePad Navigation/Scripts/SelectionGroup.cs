//
//  SelectionGroup.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

#define JIKKOU_UI_CSHARP_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR C# EVENTS ----- \\
#define JIKKOU_UI_UNITY_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR UNITY EVENTS ----- \\
//#define JIKKOU_UI_BOOLEAN_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR ADDITIONNAL BOOLEAN EVENTS ----- \\
//#define JIKKOU_UI_NESTED_GROUPS // ----- UNDER WORK, DO NOT UNCOMMENT FOR NOW ----- \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/* Selection Groups.
 * TODO : Add Support for Nested Groups
 * TODO : Add Support for Animator Triggers (without conflict with ScrollView Animator Triggers)
 * TODO : Add method to add selectables without calling Init again
 * DONE : Add a Current Selector static accessor, to exit current when entering a selector
 * TODO : Add Option to keep selection highlighted on Deselect
 * 
 * March 20, 2017
 * New Feature : Auto Exit Explicit Mode
 * New Feature : if exitNavigationTarget is left empty, selector cannot exit with a cancel action
 * Fix : Entering a Selector will exit currently selected Selector
 * March 29, 2017
 * Renamed SubSelectorElement to SelectionGroupElement, and SubSelector to SelectionGroup
 */

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[AddComponentMenu ("UI/GamePad Navigation/Selection Group")]
	[RequireComponent (typeof (Selectable))]
	[DisallowMultipleComponent]
	[SelectionBase]
	/// <summary>
	/// Selection Group.
	/// </summary>
	public class SelectionGroup : UIBehaviour, ISelectHandler, ISubmitHandler, IDeselectHandler, IPointerClickHandler
	{
		[Tooltip ("The Selectable Content Container")]
		public RectTransform content;
		[Tooltip ("The Default Selectable Object.\nDefaults to first found in Content.")]
		[SerializeField] private Selectable _defaultSelection;
		public Selectable DefaultSelection
		{
			get
			{
				if (_defaultSelection == null || !Selectables.Contains (_defaultSelection))
					_defaultSelection = Selectables[0];
				
				return _defaultSelection;
			}
			set
			{
				if (_defaultSelection != value)
					_defaultSelection = value;
			}
		}

		[Tooltip ("Initialise the Selection Group on Start.\n" +
			"Can be disabled if View content is populated by another component that will initialise the view.")]
		public bool initOnStart = true;

		[Tooltip ("Enter Group when selected.\n" +
			"Turn off to manually enter OnSubmit.")]
		public bool enterOnSelect = true;

		[Tooltip ("Exit Group when edge is reached.\n" +
			"None : Never Exit\n" +
			"Horizontal : Exit if moving left/right and no selectable is found\n" +
			"Vertical : Exit if moving up/down and no selectable is found\n" +
			"Automatic : Exit if moving in any direction and no selectable is found\n" +
			"Explicit : Exit if moving in explicit direction and no selectable is found.")]
		public Navigation.Mode autoExitNavigationMode = Navigation.Mode.None;
		public bool exitLeft;
		public bool exitRight;
		public bool exitUp;
		public bool exitDown;

		[Tooltip ("Automatically Select Neighbour on Auto Exit.")]
		public bool followNavigationOnExit = true;

		[Tooltip ("Exit Selectable Target. Left blank, makes the selector not exitable on Cancel.")]
		public Selectable exitNavigationTarget;

		public enum SelectionSaveMode { OnSubmit, OnExit, Never };
		[Tooltip ("Saves Default Selection on Submit, Exit or Never.")]
		public SelectionSaveMode selectionSaveMode;

		#region Unity Events
		#if JIKKOU_UI_UNITY_EVENTS
		[System.Serializable]
		public class IntEvent : UnityEvent <int> {}

		[Header ("Events")]
		[Tooltip ("Selected and Deselected events will not be raised when current selection is within group on Init.")]
		public bool silentOnInit = true;
		bool silent;

		[Tooltip ("The Unity Event invoked when selection changes.")]
		/// <summary>
		/// The Unity Event invoked when selection changes.
		/// </summary>
		public IntEvent onSelectionChanged;

		[Tooltip ("The Unity Event invoked when the group is entered.")]
		/// <summary>
		/// The Unity Event invoked when the group is entered.
		/// </summary>
		public UnityEvent onEnter;
		[Tooltip ("The Unity Event invoked when the group is exited.")]
		/// <summary>
		/// The Unity Event invoked when the group is exited.
		/// </summary>
		public UnityEvent onExit;

		[Tooltip ("The Unity Event invoked when the group is submitted a selection.")]
		/// <summary>
		/// The Unity Event invoked when the group is submitted a selection.
		/// </summary>
		public IntEvent onSubmit;
		#endif

		#if JIKKOU_UI_BOOLEAN_EVENTS
		[System.Serializable]
		public class BoolEvent : UnityEvent <bool> {}

		[Header ("Returns true when the view is selected, false when it is deselected.")]
		[Tooltip ("The Unity Event invoked when the view is selected. (Returns true if selected)")]
		/// <summary>
		/// The Unity Event invoked when the view is selected. (Returns true if selected)
		/// </summary>
		public BoolEvent onEnterAsBoolean;

		[Header ("Returns true when the view is deselected, false when it is selected.")]
		[Tooltip ("The Unity Event invoked when the view is deselected. (Returns true if deselected)")]
		/// <summary>
		/// The Unity Event invoked when the view is deselected. (Returns true if deselected)
		/// </summary>
		public BoolEvent onExitAsBoolean;

		#endif
		#endregion

		#region C-Sharp Events
		#if JIKKOU_UI_CSHARP_EVENTS

		public delegate void SelectionChange(int selection);
		public event SelectionChange SelectionChanged;

		public delegate void SelectionState();
		public event SelectionState SelectionEnter;
		public event SelectionState SelectionExit;

		public delegate void SelectionSubmit(int selection);
		/// <summary>
		/// Occurs when a selection is submitted.
		/// </summary>
		public event SelectionSubmit SelectionSubmitted;

		public delegate void SelectorInitialisation ();
		/// <summary>
		/// Occurs when View is initialised.
		/// </summary>
		public event SelectorInitialisation SelectorInitialised;

		#endif
		#endregion

		#if JIKKOU_UI_NESTED_GROUPS
		public delegate void GroupsChange ();
		static private event GroupsChange GroupsChanged;

		static private List<SelectionGroup> _allGroups;
		static public List<SelectionGroup> AllGroups
		{
			get
			{
				if (_allGroups == null)
					_allGroups = new List<SelectionGroup> (FindObjectsOfType<SelectionGroup>());
				
				return _allGroups;
			}
			set
			{
				if (_allGroups != value)
				{
					_allGroups = value;

					if (GroupsChanged != null)
						GroupsChanged ();
				}
			}
		}
		#endif

		private static SelectionGroup _currentGroup;

		private Selectable _selectable;
		private bool _alreadySelected;

		private List<Selectable> _selectables;
		public List<Selectable> Selectables
		{
			get
			{
				if (_selectables == null || _selectables.Count == 0)
				{
					_selectables = new List<Selectable> (content.GetComponentsInChildren<Selectable>(false));
					if (_selectables.Contains(_selectable))
						_selectables.Remove(_selectable);
				}

				if (_selectables.Count == 0)
					Debug.LogError ("No Selectables Found in View Content. Call SelectionGroup.Init() after populating the View content.");
				
				return _selectables;
			}
			private set
			{
				if (value != _selectables)
				{
					_selectables = value;
				}
			}
		}

		private Selectable _currentSelection;
		public Selectable CurrentSelection
		{
			get
			{
				if (_currentSelection == null)
					_currentSelection = DefaultSelection;

				return _currentSelection;
			}
			set
			{
				if (_currentSelection != value)
				{
					_currentSelection = value;

					#if JIKKOU_UI_CSHARP_EVENTS
					if (SelectionChanged != null)
						SelectionChanged (SelectionIndex);
					#endif

					#if JIKKOU_UI_UNITY_EVENTS
					onSelectionChanged.Invoke (SelectionIndex);
					#endif
				}
			}
		}

		public int SelectionIndex
		{
			get { return Selectables.FindIndex (x => x== CurrentSelection); }
		}

		private bool _selectedState;
		public bool SelectedState
		{
			get { return _selectedState; }
			set
			{
				if (value != _selectedState)
				{
					_selectedState = value;

					if (_selectedState)
					{
						#if JIKKOU_UI_CSHARP_EVENTS
						if (SelectionEnter != null)
						SelectionEnter ();
						#endif

						if (!silent)
						{
							#if JIKKOU_UI_BOOLEAN_EVENTS
							onExitAsBoolean.Invoke (false);
							onEnterAsBoolean.Invoke (true);
							#endif

							#if JIKKOU_UI_UNITY_EVENTS
							onEnter.Invoke ();
							#endif
						}
					}
					else
					{
						#if JIKKOU_UI_CSHARP_EVENTS
						if (SelectionExit != null)
						SelectionExit ();
						#endif

						if (!silent)
						{
							#if JIKKOU_UI_BOOLEAN_EVENTS
							onEnterAsBoolean.Invoke (false);
							onExitAsBoolean.Invoke (true);
							#endif

							#if JIKKOU_UI_UNITY_EVENTS
							onExit.Invoke ();
							#endif
						}
					}

					silent = false;
				}
			}
		}

		private SelectionScrollView _thisSelectionScrollView;
		public SelectionScrollView ThisSelectionScrollView
		{
			get
			{
				if (_thisSelectionScrollView == null)
					_thisSelectionScrollView = content.GetComponent<SelectionScrollView>();

				if (_thisSelectionScrollView == null)
					_thisSelectionScrollView = GetComponent<SelectionScrollView>();
				
				return _thisSelectionScrollView;
			}
		}

		private List<Selectable> _previousSelectables;

		// Selector Select Event Handler
		public void OnSelect (BaseEventData data)
		{
			// FIXME : Select and PointerClick events both happen on a click

			if (enterOnSelect && !_alreadySelected)
				StartCoroutine (EnterAtEndOfFrame ());
//			else if (_alreadySelected)
//				ExecuteEvents.Execute(_selectable.gameObject, data, ExecuteEvents.selectHandler); // this crashes Unity!
			// FIXME : when already selected, selectable state doesn't update
		}

		// Selector PointerClick Event Handler
		public void OnPointerClick (PointerEventData data)
		{
			if (!SelectedState)
				Enter (true, true);
		}

		// Selector Deselect Event Handler
		public void OnDeselect (BaseEventData data)
		{
			_alreadySelected = false;
		}

		// Selector Submit Event Handler
		public void OnSubmit (BaseEventData data)
		{
			Enter ();
		}

		/// <summary>
		/// Select the specified selection.
		/// </summary>
		/// <param name="selection">Selection.</param>
		public void Select (Selectable selection)
		{
			CurrentSelection = selection;
		}

		public void SubmitSelection (Selectable selection)
		{
			if (selectionSaveMode == SelectionSaveMode.OnSubmit && selection != null)
				DefaultSelection = selection;

			#if JIKKOU_UI_CSHARP_EVENTS
			if (SelectionSubmitted != null)
				SelectionSubmitted (SelectionIndex);
			#endif

			#if JIKKOU_UI_UNITY_EVENTS
			onSubmit.Invoke (SelectionIndex);
			#endif
		}

		/// <summary>
		/// Enter this Group.
		/// Used for Unity Event (no parameter).
		/// </summary>
		public void Enter ()
		{
			Enter (false, false);
		}

		/// <summary>
		/// Enters this Group.
		/// </summary>
		/// <param name="forceSelectEvent">If set to <c>true</c> forces selectHandler on selection.</param>
		/// <param name="isPointerClick">If set to <c>true</c> doesn't forces deselectHandler on selection.</param>
		public void Enter (bool forceSelectEvent = false, bool isPointerClick = false)
		{
			if (_currentGroup != null)
			{
				_currentGroup.Exit (_currentGroup.CurrentSelection, MoveDirection.None, true); // force exit current selector if any
			}

			_previousSelectables = new List<Selectable> ();

			// removing sub items from previously selectable items, but not groups
			foreach (Selectable s in Selectable.allSelectables)
				if (s.GetComponent<SelectionGroupElement>() == null || s.GetComponent<SelectionGroup>() != null)
					_previousSelectables.Add (s);

			// disabling previously selectable items
			foreach (Selectable s in Selectable.allSelectables)
				s.interactable = false;

			BaseEventData data = new BaseEventData (EventSystem.current);
			// enabling selectable items
			foreach (Selectable s in Selectables)
			{
				s.interactable = true;
				// Force Deselect All
				ExecuteEvents.Execute (s.gameObject, data, ExecuteEvents.deselectHandler);
			}

			// selecting default selection if not clicked
			if (!isPointerClick)
				DefaultSelection.Select ();

			if (forceSelectEvent || isPointerClick)
			{
				if (isPointerClick)
					ExecuteEvents.Execute (DefaultSelection.gameObject, data, ExecuteEvents.deselectHandler);
				else
					ExecuteEvents.Execute (DefaultSelection.gameObject, data, ExecuteEvents.selectHandler);
			}

			SelectedState = true;

			_currentGroup = this;
		}

		/// <summary>
		/// Exits this Group.
		/// Used for Unity Event (no jump direction).
		/// </summary>
		/// <param name="currentSelection">Pass the Current selection to be saved.</param>
		public void Exit (Selectable selection = null)
		{
			Exit (selection, MoveDirection.None, false);
		}

		/// <summary>
		/// Exits this Group.
		/// </summary>
		/// <param name="currentSelection">Pass the Current selection to be saved.</param>
		/// <param name="direction">Pass the MoveDirection to find a Selectable.</param>
		public void Exit (Selectable selection = null, MoveDirection direction = MoveDirection.None, bool forceExit = false)
		{
			if (exitNavigationTarget == null && direction == MoveDirection.None && !forceExit) // (!followNavigationOnExit || direction == MoveDirection.None))
				return;
			
			if (ThisSelectionScrollView != null)
				ThisSelectionScrollView.Deselect ();

			_alreadySelected = true;

			Selectable sel = selection != null ? selection : CurrentSelection;

//			if (selectionSaveMode == SelectionSaveMode.OnExit && selection != null)
//				DefaultSelection = selection;
			if (selectionSaveMode == SelectionSaveMode.OnExit && sel != null)
				DefaultSelection = sel;

			// enabling other selectables
			foreach (Selectable s in _previousSelectables)
				s.interactable = true;
			
			// disabling sub items
			foreach (Selectable s in Selectables)
				s.interactable = false;

			SelectedState = false;

			Selectable target = null;

			if (followNavigationOnExit && direction != MoveDirection.None)
			{
				switch (direction)
				{
					case MoveDirection.Left:
						target = _selectable.FindSelectableOnLeft();
						break;
					case MoveDirection.Right:
						target = _selectable.FindSelectableOnRight();
						break;
					case MoveDirection.Up:
						target = _selectable.FindSelectableOnUp();
						break;
					case MoveDirection.Down:
						target = _selectable.FindSelectableOnDown();
						break;
					default:
						target = _selectable; // selecting top group
						break;
				}
				if (target == null)
					target = exitNavigationTarget != null ? exitNavigationTarget : _selectable;
			}
			else if (exitNavigationTarget != null) // else if (direction == MoveDirection.None && exitNavigationTarget != null)
			{
				target = exitNavigationTarget;
			}

			if (target != null)
				target.Select ();

			if (target != _selectable)
			{
				// force deselection if target is not the selector
				BaseEventData data = new BaseEventData (EventSystem.current);
				ExecuteEvents.Execute (_selectable.gameObject, data, ExecuteEvents.deselectHandler);
			}

			if (ThisSelectionScrollView != null)
			{
				ThisSelectionScrollView.Deselect ();
			}

			_currentGroup = null;
		}

		/// <summary>
		/// Init the Selection Group Items.
		/// Must be called after populating the Sub Selector content
		/// If a Selection ScrollView is on the same GameObject, Init is automatically subscribed to the SSView Init event,
		/// no need to call Init on the Group then.
		/// </summary>
		public void Init ()
		{
			Selectables = new List<Selectable> (content.GetComponentsInChildren<Selectable>(true));

			// remove this group from the selectables if Selector sits on Content Game GameObject
			if (_selectables.Contains(_selectable))
				_selectables.Remove(_selectable);

			Selectable currentSelection = null;
			bool currentSelectionWithinSelector = false;
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				currentSelection = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
				currentSelectionWithinSelector = (Selectables.Contains (currentSelection));
			}
			else if (initOnStart && EventSystem.current.firstSelectedGameObject != null)
			{
				currentSelection = EventSystem.current.firstSelectedGameObject.GetComponent<Selectable>();
				currentSelectionWithinSelector = (Selectables.Contains (currentSelection));
			}

			foreach (Selectable s in Selectables)
			{
				SelectionGroupElement item = s.GetComponent<SelectionGroupElement>();
				if (item == null)
					item = s.gameObject.AddComponent<SelectionGroupElement>();
				
				item.Group = this;

				s.interactable = false;
			}

//			if (exitNavigationTarget == null) // This is now done in Reset. If none is assigned, selector cannot exit with a cancel action.
//				exitNavigationTarget = _selectable;

			// if current selection is within selector items
			if (currentSelectionWithinSelector)
			{
				DefaultSelection = currentSelection;

				silent = silentOnInit;

				// TODO : set is pointer click based on current Input Module
				StartCoroutine (EnterAtEndOfFrame (true));
			}

			if (SelectorInitialised != null)
				SelectorInitialised ();
		}

		private IEnumerator EnterAtEndOfFrame (bool forceSelectEvent = false, bool isPointerClick = false)
		{
			yield return new WaitForEndOfFrame ();
			Enter (forceSelectEvent, isPointerClick);
		}

		#region UIBehaviour Override Methods
		protected override void Awake ()
		{
			_selectable = GetComponent<Selectable>();

			if (ThisSelectionScrollView != null)
				ThisSelectionScrollView.ViewInitialised += Init;
		}

		protected override void Start ()
		{
			// Init On Start if required and Selection Scroll View not present, or Selection Scroll View doesn't init on start
			if (initOnStart && (ThisSelectionScrollView == null || !ThisSelectionScrollView.initOnStart))
				Init ();
		}

		#if UNITY_EDITOR
		protected override void Reset ()
		{
			ScrollRect sr = GetComponent<ScrollRect>();
			if (sr && sr.content)
				content = sr.content;
			else
				content = GetComponent<RectTransform>();

			exitNavigationTarget = _selectable;
		}
		#endif
		#endregion
	}
}