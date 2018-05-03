//
//  ScrollingListView.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

#define JIKKOU_UI_CSHARP_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR C# EVENTS ----- \\
#define JIKKOU_UI_UNITY_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR UNITY EVENTS ----- \\
//#define JIKKOU_UI_BOOLEAN_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR ADDITIONNAL BOOLEAN EVENTS ----- \\
//#define JIKKOU_UI_LOOP_CONTENT // ----- WORK IN PROGRESS, UNCOMMENT AT YOUR OWN RISK ----- \\
//#define JIKKOU_UI_GRIDLAYOUT // ----- WORK IN PROGRESS, UNCOMMENT AT YOUR OWN RISK ----- \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;

// * Navigable Scroll View.
// 
// TODO : Add Support for Grid Layout Groups
// UNDONE : Add Option for loopable content
// DONE : Add Option for default selection
// UNDONE : Add Option for Selection Alignment (Center)
// DONE : Add UNDO Support for Inspector Actions
// DONE : Test Inspector Actions
// 

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[AddComponentMenu ("UI/GamePad Navigation/Scrolling List")]
	[DisallowMultipleComponent]
	[RequireComponent (typeof (ScrollRect))]
	/// <summary>
	/// Navigable Scroll View.
	/// </summary>
	public class ScrollingListView : Selectable, ISubmitHandler, ICancelHandler, IScrollHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
	{
		public enum ScrollingMode {Auto, Vertical, Horizontal};
		[Tooltip ("Scrolling Mode : \n" +
			"Auto : Looks for a Vertical/Horizontal Layout Group on the Scroll View content.\n" +
			"Vertical : Vertical Navigation through Selectable objects found in Scroll View content.\n" +
			"Horizontal : Horizontal Navigation through Selectable objects found in Scroll View content.")]
		[FormerlySerializedAs("mode")]
		[SerializeField] ScrollingMode scrollingMode;
		ScrollViewHelper.ScrollingMode _scrollingMode; // filtered scrolling mode

		public enum Alignment {None, Min, Center, Max, Pivot};

		[Header ("Alignment")]
		[Tooltip ("Alignement : \n" +
			"None : no alignement\n +" +
			"Min : aligns Element Left (or Bottom) edge to Guide Left (or Bottom) edge.\n +" +
			"Center : aligns Element center to Guide center. \n" +
			"Max : aligns Element Right (or Top) edge to Guide Right (or Top) edge.\n" +
			"Pivot : aligns Element Pivot to Guide Pivot")]
		[SerializeField] Alignment alignment;
		ScrollViewHelper.Alignment _alignmentMode; // filtered alignment mode

		[Tooltip ("The RectTransform to align selection to.")]
		public RectTransform alignmentGuide;
		[Tooltip ("Forces scrolling when content already fits viewport.\n" +
			"NOTE : this disables the ScrollRect component.")]
		public bool forceScrolling;
		[Tooltip ("Elements transforms are read every frame during scrolling.\n" +
			"Use when elements transforms animate during transitions.")]
		public bool updateTransforms;
		[Tooltip ("Elements transforms are read every frame all the time.\n" +
			"Use when elements transforms animate after transitions.")]
		public bool alwaysUpdate;

		[Header ("Animation")]
		[Tooltip ("The time it takes the view to scroll to selected content.")]
		public float navigationScrollingTime = 0.5f;
		[Tooltip ("Curve to control scrolling time.")]
		public AnimationCurve navigationScrollingCurve = new AnimationCurve (new Keyframe [2] {new Keyframe(0,0), new Keyframe(1, 1)});

		[Header ("Behaviour")]
		[Tooltip ("Initialise the Scroll View on Start.\n" +
			"Can be disabled if View content is populated by another component that will initialise the view.")]
		public bool initOnStart = true;

		[Tooltip ("Deselect selected content when exiting the view.\n" +
			"Turn off when selection must stay visible at all times.")]
		public bool deselectedContentOnExit = true;

		[Tooltip ("Use Selectable Objects.\n" +
			"Turn off to use NSVItems instead. (Default : Off)")]
		public bool useSelectables = false;

		[Tooltip ("The initial selection.")]
		public int defaultSelection;

		#if JIKKOU_UI_LOOP_CONTENT
		[Tooltip ("Cycle objects order when selection changes.\n" +
			"The returned Selection remains the same.")]
		[SerializeField] bool loopContent = false;
		#endif

		[Tooltip ("How much scrolling it takes to navigate the view.")]
		[SerializeField] private float _scrollThreshold = 10f;
		[Tooltip ("How much dragging it takes to navigate the view.")]
		[SerializeField] private float _dragThreshold = 10f;

		#region Animator
		private Animator _animator;

		[Header ("Animator Parameters")]
		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when selection changes.")]
		public string selectionChangedTrigger = "SelectionChanged";
		private int _selectionChangedHashID;
		[Tooltip ("The name of the Animator Bool Parameter to be modified when view is selected/deselected.")]
		public string selectedBool = "Selected";
		private int _selectedHashID;
		[Tooltip ("The name of the Animator Int Parameter to be modified when selection changes.")]
		public string selectionInt = "Selection";
		private int _selectionHashID;

		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when selection is submitted.")]
		public string selectionSubmittedTrigger = "SelectionSubmitted";
		private int _selectionSubmittedHashID;

		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when selection is cancelled.")]
		public string selectionCancelledTrigger = "SelectionCancelled";
		private int _selectionCancelledHashID;
		#endregion

		#region Unity Events
		#if JIKKOU_UI_UNITY_EVENTS

		[System.Serializable]
		public class IntEvent : UnityEvent <int> {}

		[Header ("Events")]

		[Tooltip ("The Unity Event invoked when selection changes.")]
		/// <summary>
		/// The Unity Event invoked when selection changes.
		/// </summary>
		public IntEvent onSelectionChanged;

		[Tooltip ("The Unity Event invoked when the view is selected.")]
		/// <summary>
		/// The Unity Event invoked when the view is selected. (Returns true if selected)
		/// </summary>
		public UnityEvent onListSelected;

		[Tooltip ("The Unity Event invoked when the view is deselected.")]
		/// <summary>
		/// The Unity Event invoked when the view is deselected. (Returns true if deselected)
		/// </summary>
		public UnityEvent onListDeselected;

		[Tooltip ("The Unity Event invoked when the view submits a selection.")]
		/// <summary>
		/// The Unity Event invoked when the view submits a selection.
		/// </summary>
		public IntEvent onSubmit;

		[Tooltip ("The Unity Event invoked when the view cancels a selection.")]
		/// <summary>
		/// The Unity Event invoked when the view cancels a selection.
		/// </summary>
		public UnityEvent onCancel;

		#endif

		#if JIKKOU_UI_BOOLEAN_EVENTS

		[System.Serializable]
		public class BoolEvent : UnityEvent <bool> {}

		[Header ("Returns true when the view is selected, false when it is deselected.")]
		[Tooltip ("The Unity Event invoked when the view is selected. (Returns true if selected)")]
		/// <summary>
		/// The Unity Event invoked when the view is selected. (Returns true if selected)
		/// </summary>
		public BoolEvent onListSelectedAsBoolean;

		[Header ("Returns true when the view is deselected, false when it is selected.")]
		[Tooltip ("The Unity Event invoked when the view is deselected. (Returns true if deselected)")]
		/// <summary>
		/// The Unity Event invoked when the view is deselected. (Returns true if deselected)
		/// </summary>
		public BoolEvent onListDeselectedAsBoolean;

		#endif

		#endregion

		#region Members
		private List<Selectable> _selectables;
		public List<Selectable> Selectables
		{
			get
			{
				if (_selectables == null)
					_selectables = new List<Selectable> (Content.GetComponentsInChildren<Selectable>(false));
					
				if (_selectables.Count == 0)
					Debug.LogError ("No Selectables Found in View Content. Call SelectableScrollView.Init() after populating the View content.");
				
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

		private List<ScrollingListItemView> _selectableElements;
		public List<ScrollingListItemView> SelectableElements
		{
			get
			{
				if (_selectableElements == null)
					_selectableElements = new List<ScrollingListItemView> (Content.GetComponentsInChildren<ScrollingListItemView>(false));

				if (_selectableElements.Count == 0)
					Debug.LogError ("No Selectable Elements Found in View Content. Call SelectableScrollView.Init() after populating the View content.");

				return _selectableElements;
			}
			private set
			{
				if (value != _selectableElements)
				{
					_selectableElements = value;
				}
			}
		}

		private int Count
		{
			get { return useSelectables ? Selectables.Count : SelectableElements.Count; }
		}

		private GameObject SelectedGameObject
		{
			get { return useSelectables ? Selectables[Selection].gameObject : SelectableElements[Selection].gameObject; }
		}

		private ScrollRect _scrollRect;
		/// <summary>
		/// Returns the ScrollRect Content
		/// </summary>
		public RectTransform Content
		{
			get
			{
				if (!_scrollRect)
					_scrollRect = GetComponent<ScrollRect>();
				
				return _scrollRect.content ;
			}
		}

		private float _lerp;
		private float _lCurve;
		private Vector2 _previousPosition;
		private Vector2 _nextPosition;
		private float _timeStamp;
		private bool _moving;
		#endregion

		#region C-Sharp Events
		#if JIKKOU_UI_CSHARP_EVENTS

		public delegate void SelectionChange(int selection);
		/// <summary>
		/// Occurs when the selection changed.
		/// </summary>
		public event SelectionChange SelectionChanged;

		public delegate void SelectedChange(bool selected);
		/// <summary>
		/// Occurs when the view is selected/deselected.
		/// </summary>
		public event SelectedChange SelectedChanged;

		public delegate void SelectionSubmit(int selection);
		/// <summary>
		/// Occurs when a selection is submitted.
		/// </summary>
		public event SelectionSubmit SelectionSubmitted;

		public delegate void SelectionCancel(int selection);
		/// <summary>
		/// Occurs when a selection is cancelled.
		/// </summary>
		public event SelectionCancel SelectionCancelled;

		public delegate void Scroll(Vector2 position);
		/// <summary>
		/// Occurs when the View scrolled.
		/// </summary>
		public event Scroll Scrolled;

		public delegate void ViewInitialisation ();
		/// <summary>
		/// Occurs when View is initialised.
		/// </summary>
		public event ViewInitialisation ViewInitialised;

		#endif
		#endregion

		#if JIKKOU_UI_LOOP_CONTENT
		private int _offset;
		public int Offset
		{
			get { return _offset; }
			private set
			{
				if (value < -Count)
					_offset = Count + value;
				if (value >= Count)
					_offset = value - Count;

				if (_offset > 0)
					for (int i = 0 ; i < _offset ; i++)
						Content.GetChild(0).SetAsLastSibling();
				else
					for (int i = _offset ; i > 0 ; i--)
						Content.GetChild(Content.childCount-1).SetAsFirstSibling();
			}
		}
		#endif

		private int _selection;
		/// <summary>
		/// Returns the current selection.
		/// </summary>
		public int Selection
		{
			get { return _selection; }
			set
			{
				Selected = true;

				if (value != _selection)
				{
					#if JIKKOU_UI_LOOP_CONTENT
					if (loopContent)
					{
						if (value < 0)
							_selection = Count + value;
						else if (value >= Count)
							_selection = value - Count;
						else
							_selection = value;
					}
					else 
					#endif
					if (value < 0 || value >= Count)
					{
						return;
					}

					PointerEventData data = new PointerEventData(EventSystem.current);
					ExecuteEvents.Execute (SelectedGameObject, data, ExecuteEvents.deselectHandler);

					#if JIKKOU_UI_LOOP_CONTENT
					if (loopContent)
						Offset = value - _selection;
					#endif

					_selection = value;

					ExecuteEvents.Execute (SelectedGameObject, data, ExecuteEvents.selectHandler);

					#if JIKKOU_UI_LOOP_CONTENT
					if (!loopContent)
					{
					#endif
					if (!forceScrolling)
					{
						_nextPosition = _previousPosition = _scrollRect.normalizedPosition;
						UpdateNextPosition();

						if (navigationScrollingTime == 0)
						{
							_scrollRect.normalizedPosition = _nextPosition;
						}
						else
						{
							_timeStamp = Time.time;
							_lerp = 0;
							_moving = true;
						}
					}
					else // if forceScrolling
					{
						_nextPosition = _previousPosition = Content.localPosition;
						UpdateNextPosition();

						if (navigationScrollingTime == 0)
						{
							Content.localPosition = _nextPosition;
						}
						else
						{
							_timeStamp = Time.time;
							_lerp = 0;
							_moving = true;
						}
					}
//						_previousPosition = _scrollRect.normalizedPosition;
//						switch (mode)
//						{
//							case ScrollingMode.Vertical :
//								_nextPosition.y = 1f - (((float)_selection) / (Count - 1));
//								break;
//							case ScrollingMode.Horizontal : 
//								_nextPosition.x = (((float)_selection) / (Count - 1));
//								break;
//						}
//
//						if (navigationScrollingTime == 0)
//						{
//							_scrollRect.normalizedPosition = _nextPosition;
//						}
//						else
//						{
//							_timeStamp = Time.time;
//							_lerp = 0;
//							_moving = true;
//						}
					#if JIKKOU_UI_LOOP_CONTENT
					}
					#endif

					if (_animator)
					{
						_animator.SetInteger (_selectionHashID, _selection);
						_animator.SetTrigger (_selectionChangedHashID);
					}

					#if JIKKOU_UI_CSHARP_EVENTS
					if (SelectionChanged != null)
						SelectionChanged(_selection);
					#endif

					#if JIKKOU_UI_UNITY_EVENTS
					onSelectionChanged.Invoke (_selection);
					#endif
				}
			}
		}

		private bool _selected;
		/// <summary>
		/// Returns true if the view is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			private set
			{
				if (value != _selected)
				{
					_selected = value;

					PointerEventData data = new PointerEventData(EventSystem.current);
					if (_selected)
						ExecuteEvents.Execute (SelectedGameObject, data, ExecuteEvents.selectHandler);
					else
						if (deselectedContentOnExit)
							ExecuteEvents.Execute (SelectedGameObject, data, ExecuteEvents.deselectHandler);

					if (_animator)
					{
						_animator.SetBool (_selectedHashID, _selected);
					}

					#if JIKKOU_UI_CSHARP_EVENTS
					if (SelectedChanged != null)
						SelectedChanged(_selected);
					#endif

					#if JIKKOU_UI_UNITY_EVENTS
					if (_selected)
						onListSelected.Invoke ();
					else
						onListDeselected.Invoke ();
					#endif

					#if JIKKOU_UI_BOOLEAN_EVENTS
					onListSelectedAsBoolean.Invoke (_selected);
					onListDeselectedAsBoolean.Invoke (!_selected);
					#endif
				}
			}
		}

		public override void OnSelect (BaseEventData data)
		{
			base.OnSelect (data);
			Selected = true;
		}

		public override void OnDeselect (BaseEventData data)
		{
			base.OnDeselect (data);
			Selected = false;
		}

		private void Move (MoveDirection moveDir)
		{
			// selecting elements within the list
			switch (moveDir)
			{
				case MoveDirection.Down :
					switch (scrollingMode)
					{
						case ScrollingMode.Vertical :
							if (Selection < Count-1)
							{
								Selection++;
								return;
							}
							break;
						case ScrollingMode.Horizontal :
							break;
					}
					break;
				case MoveDirection.Up :
					switch (scrollingMode)
					{
						case ScrollingMode.Vertical :
							if (Selection > 0)
							{
								Selection--;
								return;
							}
							break;
						case ScrollingMode.Horizontal :
							break;
					}
					break;
				case MoveDirection.Left :
					switch (scrollingMode)
					{
						case ScrollingMode.Vertical :
							break;
						case ScrollingMode.Horizontal :
							if (Selection > 0)
							{
								Selection--;
								return;
							}
							break;
					}
					break;
				case MoveDirection.Right :
					switch (scrollingMode)
					{
						case ScrollingMode.Vertical :
							break;
						case ScrollingMode.Horizontal :
							if (Selection < Count-1)
							{
								Selection++;
								return;
							}
							break;
					}
					break;
			}

			// selecting neighbours if we reached the end of the list or in a different direction
			switch (moveDir)
			{
				case MoveDirection.Down :
					if (this.FindSelectableOnDown ())
						this.FindSelectableOnDown ().Select();
					break;
				case MoveDirection.Up :
					if (this.FindSelectableOnUp ())
						this.FindSelectableOnUp ().Select();
					break;
				case MoveDirection.Left :
					if (this.FindSelectableOnLeft ())
						this.FindSelectableOnLeft ().Select();
					break;
				case MoveDirection.Right :
					if (this.FindSelectableOnRight ())
						this.FindSelectableOnRight ().Select();
					break;
			}
		}

		public override void OnMove (AxisEventData data) 
		{
			Move (data.moveDir);
		}

		private MoveDirection VectorToMoveDirection (Vector2 vector)
		{
			MoveDirection direction = MoveDirection.None;

			if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
				direction = vector.x > 0 ? MoveDirection.Right : MoveDirection.Left;
			else
				direction = vector.y > 0 ? MoveDirection.Up : MoveDirection.Down;

			return direction;
		}

		private Vector2 _scroll;
		private bool _isScrolling;
		public void OnScroll (PointerEventData data)
		{
			// scrollDelta never comes back to Vector2.zero
			// IsScrolling always returns true :/
			_isScrolling = true; // Scrolling Stop handling moved to Update

			_scroll.x -= data.scrollDelta.x;
			_scroll.y += data.scrollDelta.y;
			if ((scrollingMode == ScrollingMode.Horizontal && Mathf.Abs(_scroll.x) <= _scrollThreshold) ||
				(scrollingMode == ScrollingMode.Vertical && Mathf.Abs(_scroll.y) <= _scrollThreshold))
				return;
				
				Move (VectorToMoveDirection (_scroll));
				_scroll = Vector2.zero;
//			}
		}

		private Vector2 _drag;
		public void OnBeginDrag (PointerEventData data)
		{
			_drag = Vector2.zero;
		}

		public void OnDrag (PointerEventData data)
		{
			_drag -= data.delta;

			if ((scrollingMode == ScrollingMode.Horizontal && Mathf.Abs(_drag.x) <= _dragThreshold) ||
				(scrollingMode == ScrollingMode.Vertical && Mathf.Abs(_drag.y) <= _dragThreshold))
				return;

			Move (VectorToMoveDirection (_drag));
			_drag = Vector2.zero;
		}

		public void OnEndDrag (PointerEventData data)
		{
			_drag = Vector2.zero;
			OnSubmit (data);
		}

		public void OnPointerClick (PointerEventData data)
		{
			OnSubmit (data);
		}

		public void OnSubmit (BaseEventData data)
		{
			// set animator trigger
			if (_animator)
				_animator.SetTrigger (_selectionSubmittedHashID);

			#if JIKKOU_UI_CSHARP_EVENTS
			if (SelectionSubmitted != null)
				SelectionSubmitted (Selection);
			#endif

			// execute selectable submit eventhandler
			ExecuteEvents.Execute (SelectedGameObject, data, ExecuteEvents.submitHandler);

			#if JIKKOU_UI_UNITY_EVENTS
			onSubmit.Invoke (Selection);
			#endif
		}

		public void OnCancel (BaseEventData data)
		{
			// set animator trigger
			if (_animator)
				_animator.SetTrigger (_selectionCancelledHashID);
			
			#if JIKKOU_UI_CSHARP_EVENTS
			if (SelectionCancelled != null)
				SelectionCancelled (Selection);
			#endif

			// execute selectable cancel eventhandler
			ExecuteEvents.Execute (SelectedGameObject, data, ExecuteEvents.cancelHandler);

			#if JIKKOU_UI_UNITY_EVENTS
			onCancel.Invoke ();
			#endif
		}

		/// <summary>
		/// Init the Selectable Scroll View.
		/// Must be called after populating the Scroll View content
		/// </summary>
		public void Init ()
		{
			_scrollingMode = (ScrollViewHelper.ScrollingMode) System.Enum.Parse(typeof(ScrollViewHelper.ScrollingMode), scrollingMode.ToString());
			_alignmentMode = (ScrollViewHelper.Alignment) System.Enum.Parse(typeof(ScrollViewHelper.Alignment), alignment.ToString());

			if (useSelectables)
			{
				Selectables = new List<Selectable> (Content.GetComponentsInChildren<Selectable>(false));

				Navigation nav = new Navigation ();
				nav.mode = Navigation.Mode.None;
				foreach (Selectable s in Selectables)
					s.navigation = nav;
			}
			else
			{
				SelectableElements = new List<ScrollingListItemView> (Content.GetComponentsInChildren<ScrollingListItemView>(false));
			}

			_scrollRect.normalizedPosition = Vector2.up;

			if (defaultSelection > -1 && !deselectedContentOnExit)
				Selection = defaultSelection;
			else if (defaultSelection > -1)
				_selection = defaultSelection; // init selection silently

			if (ViewInitialised != null)
				ViewInitialised ();
		}

		protected override void Awake ()
		{
			base.Awake ();

			_scrollRect = GetComponent<ScrollRect>();
			_animator = GetComponent<Animator>();
			_selectionChangedHashID = Animator.StringToHash(selectionChangedTrigger);
			_selectedHashID = Animator.StringToHash(selectedBool);
			_selectionHashID = Animator.StringToHash(selectionInt);
			_selectionSubmittedHashID = Animator.StringToHash(selectionSubmittedTrigger);
			_selectionCancelledHashID = Animator.StringToHash(selectionCancelledTrigger);

			if (scrollingMode == ScrollingMode.Auto)
			{
				if (Content.GetComponent<VerticalLayoutGroup>() != null)
					scrollingMode = ScrollingMode.Vertical;
				else if (Content.GetComponent<HorizontalLayoutGroup>() != null)
					scrollingMode = ScrollingMode.Horizontal;
				else if (_scrollRect.vertical != _scrollRect.horizontal)
				{
					scrollingMode = _scrollRect.vertical ? ScrollingMode.Vertical : ScrollingMode.Horizontal;
				}
				else
				{
					Debug.LogWarning ("Selectable List Auto Mode requires a Layout Component on Content Object.\n" +
						"or, the Horizontal OR Vertical property set.\nUsing Vertical by default", this);
					scrollingMode = ScrollingMode.Vertical;
				}
			
			}
			// disabling Scroll Rect Scrolling as it is handled by the view itself
			_scrollRect.scrollSensitivity = 0;
			_scrollRect.horizontal = _scrollRect.vertical = false;
		}

		protected override void Start ()
		{
			base.Start();

			if (initOnStart)
				Init ();
		}

		private void Update ()
		{
			if (!_moving)
				return;

			if (_isScrolling)
			{
				PointerEventData data = new PointerEventData (EventSystem.current);
				_isScrolling = data.IsScrolling();

				OnSubmit (data);
			}

			_lerp += Time.deltaTime/navigationScrollingTime;

			_lCurve = navigationScrollingCurve.Evaluate (_lerp);

			_scrollRect.normalizedPosition = Vector2.Lerp(_previousPosition, _nextPosition, _lCurve);

			#if JIKKOU_UI_CSHARP_EVENTS
			if (Scrolled != null)
				Scrolled (_scrollRect.normalizedPosition);
			#endif

			if (Time.time >= _timeStamp+navigationScrollingTime)
				_moving = false;
		}

		private void UpdateNextPosition ()
		{
			if (_selection < 0)
				return;

			RectTransform e = useSelectables ? (RectTransform)Selectables[_selection].transform : (RectTransform)SelectableElements[_selection].transform;

			if (alignment == Alignment.None)
			{
				int count = useSelectables ? Selectables.Count : SelectableElements.Count;
				ScrollViewHelper.SetNormalisedPosition (
					ref _nextPosition,
					_selection,
					count,
					_scrollingMode
				);
			}
			else if (!forceScrolling)
			{
				ScrollViewHelper.SetNormalisedPosition (
					ref _nextPosition,
					_scrollRect.viewport,
					Content,
					e,
					alignmentGuide,
					_scrollingMode,
					_alignmentMode
				);
			}
			else
			{
				ScrollViewHelper.SetDeltaPosition (
					ref _nextPosition,
					_scrollRect.viewport,
					e,
					alignmentGuide,
					_scrollingMode,
					_alignmentMode
				);
			}
		}

		#if UNITY_EDITOR
		protected override void Reset ()
		{
			base.Reset ();

			Keyframe[] ks = new Keyframe[2];
			ks[0] = new Keyframe( 0, 0 );
			ks[0].outTangent = 2f;
			ks[1] = new Keyframe( 1, 1 );
			ks[1].outTangent = 0;
			navigationScrollingCurve = new AnimationCurve (ks);
		}

		public void AddNavigationScrollViewElementToSelectableChildren ()
		{
			int undoGroup = UnityEditor.Undo.GetCurrentGroup();
			UnityEditor.Undo.SetCurrentGroupName ("Add Navigable ScrollView Elements");
			foreach (Transform t in Content.GetComponentsInChildren<Transform>(false))
				UnityEditor.Undo.AddComponent<ScrollingListItemView>(t.gameObject);

			UnityEditor.Undo.CollapseUndoOperations (undoGroup);
		}

		public void ConvertSelectablesToNavigableScrollViewElements ()
		{
			int undoGroup = UnityEditor.Undo.GetCurrentGroup();
			UnityEditor.Undo.SetCurrentGroupName ("Convert Selectables To Navigable ScrollView Elements");
			foreach (Selectable s in Content.GetComponentsInChildren<Selectable>(false))
				ScrollingListItemView.ConvertSelectableToNSVE (s);

			UnityEditor.Undo.CollapseUndoOperations (undoGroup);
		}

		#endif
	}
}