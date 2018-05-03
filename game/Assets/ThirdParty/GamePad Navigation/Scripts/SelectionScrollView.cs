//
//  SelectionScrollView.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

#define JIKKOU_UI_CSHARP_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR C# EVENTS ----- \\
#define JIKKOU_UI_UNITY_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR UNITY EVENTS ----- \\
//#define JIKKOU_UI_BOOLEAN_EVENTS // ----- UNCOMMENT TO ADD SUPPORT FOR ADDITIONNAL BOOLEAN EVENTS ----- \\
//#define JIKKOU_UI_SSV_GRID_LAYOUT // ----- UNCOMMENT TO ADD SUPPORT FOR GRID LAYOUT ----- \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;

/* Selection Scroll View.
 * TODO : Add Support for Grid Layout Groups
 * TODO : Add Option for loopable content
 * DONE : Add Option for Selection Alignment (Center)
 * TODO : Add Option to keep selection highlighted on Deselect
 * 
 * March 20, 2017
 * New Feature : Selection Alignement
 * 
 */

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[AddComponentMenu ("UI/GamePad Navigation/Selection ScrollView")]
	[RequireComponent (typeof (ScrollRect))]
	[DisallowMultipleComponent]
	/// <summary>
	/// Selection Scroll View.
	/// </summary>
	public class SelectionScrollView : MonoBehaviour
	{
		public enum ScrollingMode {Auto, Vertical, Horizontal
			#if JIKKOU_UI_SSV_GRID_LAYOUT
			, Grid};
			#else
		};
			#endif
		[Tooltip ("Scrolling Mode : \n" +
			"Auto : Looks for a Vertical/Horizontal Layout Group on the Scroll View content.\n" +
			"Vertical : Vertical Navigation through Selectable objects found in Scroll View content.\n" +
			"Horizontal : Horizontal Navigation through Selectable objects found in Scroll View content.\n" +
			"Grid (Experimental) : Grid Navigation through Selectable objects found in Scroll View content. Requires alignment guide and force scrolling.")]
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

		[Tooltip ("Initialise the Scroll View on Start.\n" +
			"Can be disabled if View content is populated by another component that will initialise the view.")]
		public bool initOnStart = true;

		[Tooltip ("Adds Selection ScrollView Element Component to Selectables found in content on Init.\n" +
			"Can be disabled to manually select which Selectable object belong to the View.")]
		public bool autoAddElementComponentToSelectablesOnInit = true;

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

		#region Members
		private List<SelectionScrollViewElement> _selectables;
		public List<SelectionScrollViewElement> Selectables
		{
			get
			{
				if (_selectables == null || _selectables.Count == 0)
					Debug.LogError ("No Selection Scroll View Element Found in View Content. Call SelectionScrollView.Init() after populating the View content.");

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

		#endregion

		#region Unity Events
		#if JIKKOU_UI_UNITY_EVENTS
		[System.Serializable]
		public class IntEvent : UnityEvent <int> {}

		[Header ("Events")]
		[Tooltip ("Selected and Selection Changed events will not be raised when current selection is within Selection ScrollView on Init.")]
		public bool silentOnInitialSelection = true;
		bool silent;

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

		private int _selection = -1; // defaults to -1 to force position update upon init
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
					_selection = value;

//					RectTransform elementTx = (RectTransform)Selectables[_selection].transform;

					if (!forceScrolling)
					{
						_nextPosition = _previousPosition = _scrollRect.normalizedPosition;
//						_previousPosition = _scrollRect.normalizedPosition;
						UpdateNextPosition();
						
						if (navigationScrollingTime == 0)
						{
							_scrollRect.normalizedPosition = _nextPosition;
						}
						else
						{
							_timeStamp = Time.time;
							_lerp = 0;
							enabled = true;
						}
					}
					else // if forceScrolling
					{
						_nextPosition = _previousPosition = Content.localPosition;
//						_previousPosition = Content.localPosition;
						UpdateNextPosition();

						if (navigationScrollingTime == 0)
						{
							Content.localPosition = _nextPosition;
						}
						else
						{
							_timeStamp = Time.time;
							_lerp = 0;
							enabled = true;
						}
					}

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
					if (!silent)
					{
						onSelectionChanged.Invoke (_selection);
					}
					#endif
				}
			}
		}

		private bool _selected;
		/// <summary>
		/// Returns true if the currently selected Selectable is in the View.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (value != _selected)
				{
					_selected = value;

					if (_animator)
						_animator.SetBool (_selectedHashID, _selected);

					#if JIKKOU_UI_CSHARP_EVENTS
					if (SelectedChanged != null)
						SelectedChanged(_selected);
					#endif

					if (!silent)
					{
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
		}

		/// <summary>
		/// Adds SelectionScrollSiewElement to selectable children.
		/// </summary>
		/// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
		public void AddSelectionScrollViewElementToSelectableChildren (bool includeInactive = false)
		{
			foreach (Selectable s in _scrollRect.content.GetComponentsInChildren<Selectable>(includeInactive))
				if (s.GetComponent<SelectionScrollViewElement>() == null)
					s.gameObject.AddComponent<SelectionScrollViewElement>();
		}

		/// <summary>
		/// Init the Selectable Scroll View.
		/// Must be called after populating the Scroll View content
		/// </summary>
		public void Init (bool includeInactive = false)
		{
			_scrollingMode = (ScrollViewHelper.ScrollingMode) System.Enum.Parse(typeof(ScrollViewHelper.ScrollingMode), scrollingMode.ToString());
			_alignmentMode = (ScrollViewHelper.Alignment) System.Enum.Parse(typeof(ScrollViewHelper.Alignment), alignment.ToString());

			StartCoroutine (LateInit (includeInactive));
		}

		private IEnumerator LateInit (bool includeInactive = false)
		{
			// wait for full canvas initialisation
			if (forceScrolling)
				yield return new WaitForEndOfFrame();

			if (autoAddElementComponentToSelectablesOnInit)
				AddSelectionScrollViewElementToSelectableChildren (includeInactive);

			Selectables = new List<SelectionScrollViewElement> (_scrollRect.content.GetComponentsInChildren<SelectionScrollViewElement>(includeInactive));

			if (alignment == Alignment.None)
				forceScrolling = false; // in case it got switched on then changed back to Alignment.None

			if (!forceScrolling)
			{
				_scrollRect.normalizedPosition = Vector2.up;
				if (alignment != Alignment.None)
					_scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
			}
			else
			{
				_scrollRect.enabled = false;
			}

			// Default Selection
			bool currentSelectionWithinSelector = (EventSystem.current.currentSelectedGameObject != null && 
				Selectables.Contains (EventSystem.current.currentSelectedGameObject.GetComponent<SelectionScrollViewElement>()));

			bool firstSelectionWithinSelector = (EventSystem.current.firstSelectedGameObject != null && 
				Selectables.Contains (EventSystem.current.firstSelectedGameObject.GetComponent<SelectionScrollViewElement>()));

			silent = silentOnInitialSelection;
			float navscrolltime = navigationScrollingTime;
			navigationScrollingTime = 0;

			if (currentSelectionWithinSelector)
				Selection = Selectables.FindIndex(x=> x.gameObject == EventSystem.current.currentSelectedGameObject);
			else if (initOnStart && firstSelectionWithinSelector)
				Selection = Selectables.FindIndex(x=> x.gameObject == EventSystem.current.firstSelectedGameObject);
			else
				Selection = 0;

			navigationScrollingTime = navscrolltime;
			silent = false;

			if (ViewInitialised != null)
				ViewInitialised ();

			if (alwaysUpdate)
				enabled = true;
		}

		public void Awake ()
		{
			_scrollRect = GetComponent<ScrollRect>();
			_animator = GetComponent<Animator>();
			_selectionChangedHashID = Animator.StringToHash(selectionChangedTrigger);
			_selectedHashID = Animator.StringToHash(selectedBool);
			_selectionHashID = Animator.StringToHash(selectionInt);
			_selectionSubmittedHashID = Animator.StringToHash(selectionSubmittedTrigger);
			_selectionCancelledHashID = Animator.StringToHash(selectionCancelledTrigger);

			if (scrollingMode == ScrollingMode.Auto)
			{
				if (_scrollRect.content.GetComponent<VerticalLayoutGroup>())
					scrollingMode = ScrollingMode.Vertical;
				else if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>())
					scrollingMode = ScrollingMode.Horizontal;
				else
				{
					Debug.LogWarning ("Selectable List Auto Mode requires a Layout Component on Content Object.\n" +
						"Using Vertical by default", this);
					scrollingMode = ScrollingMode.Vertical;
				}
			}

			// forcing anchors and pivot
			if (alignment != Alignment.None)
			{
				Vector2 anchorMin = Content.anchorMin;
				Vector2 anchorMax = Content.anchorMax;
				Vector2 pivot = Content.pivot;
				if (scrollingMode == ScrollingMode.Horizontal)
				{
					anchorMin.x = 0;
					anchorMax.x = 1;
					pivot.x = 0;
				}
				else if (scrollingMode == ScrollingMode.Vertical)
				{
					anchorMin.y = 0;
					anchorMax.y = 1;
					pivot.y = 1;
				}
				Content.anchorMin = anchorMin;
				Content.anchorMax = anchorMax;
				Content.pivot = pivot;
			}
		}

		private void Start ()
		{
			enabled = false;

			if (initOnStart)
				Init ();
		}

		public bool Contains (SelectionScrollViewElement element)
		{
			return Selectables.Contains (element);
		}

		public void Select (SelectionScrollViewElement element)
		{
			Selection = Selectables.FindIndex (x => x==element);
		}

		public void Deselect (SelectionScrollViewElement element = null)
		{
			Selected = false;
		}

		public void Submit ()
		{
			// set animator trigger
			if (_animator)
				_animator.SetTrigger (_selectionSubmittedHashID);

			#if JIKKOU_UI_CSHARP_EVENTS
			if (SelectionSubmitted != null)
				SelectionSubmitted (Selection);
			#endif

			#if JIKKOU_UI_UNITY_EVENTS
			onSubmit.Invoke (Selection);
			#endif
		}

		public void Cancel ()
		{
			// set animator trigger
			if (_animator)
				_animator.SetTrigger (_selectionCancelledHashID);

			#if JIKKOU_UI_CSHARP_EVENTS
			if (SelectionCancelled != null)
				SelectionCancelled (Selection);
			#endif

			#if JIKKOU_UI_UNITY_EVENTS
			onCancel.Invoke ();
			#endif
		}

		public void Update ()
		{
			if (updateTransforms || alwaysUpdate)
				UpdateNextPosition();

			if (alwaysUpdate && navigationScrollingTime == 0)
			{
				if (!forceScrolling)
					_scrollRect.normalizedPosition = _nextPosition;
				else
					Content.localPosition = _nextPosition;
			}	
			else
			{
				_lerp += Time.deltaTime/navigationScrollingTime;
				_lCurve = navigationScrollingCurve.Evaluate (_lerp);

				if (!forceScrolling)
					_scrollRect.normalizedPosition = Vector2.Lerp(_previousPosition, _nextPosition, _lCurve);
				else
					Content.localPosition = Vector2.Lerp(_previousPosition, _nextPosition, _lCurve);
				
				#if JIKKOU_UI_CSHARP_EVENTS
				if (Scrolled != null)
					Scrolled (_scrollRect.normalizedPosition);
				#endif

				if (!alwaysUpdate && Time.time >= _timeStamp+navigationScrollingTime)
					enabled = false;
			}
		}

		private void Reset ()
		{
			Keyframe[] ks = new Keyframe[2];
			ks[0] = new Keyframe( 0, 0 );
			ks[0].outTangent = 2f;
			ks[1] = new Keyframe( 1, 1 );
			ks[1].outTangent = 0;
			navigationScrollingCurve = new AnimationCurve (ks);
		}

		private void UpdateNextPosition ()
		{
			if (_selection < 0)
				return;

			if (alignment == Alignment.None)
			{
				ScrollViewHelper.SetNormalisedPosition (
					ref _nextPosition,
					_selection,
					Selectables.Count,
					_scrollingMode
				);
			}
			else if (!forceScrolling)
			{
				ScrollViewHelper.SetNormalisedPosition (
					ref _nextPosition,
					_scrollRect.viewport,
					Content,
					(RectTransform)Selectables[_selection].transform,
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
					(RectTransform)Selectables[_selection].transform,
					alignmentGuide,
					_scrollingMode,
					_alignmentMode
				);
			}
		}
	}
}