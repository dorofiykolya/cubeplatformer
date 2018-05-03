//
//  SelectionGroupElement.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

/*
 * March 20, 2017
 * Bug Fix : SelectionGroupElement no longer exits when Selector.autoExitNavigationMode is set to Vertical and moving left/right, or Horizontal and moving up/down
 * Bug Fix : SubSelectorElement on Inactive GameObjects are forever disabled
 * New feature : Element autoExitNavigationMode override
 * March 29, 2017
 * Renamed SubSelectorElement to SelectionGroupElement, and SubSelector to SelectionGroup
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[DisallowMultipleComponent]
	[RequireComponent (typeof (Selectable))]
	[AddComponentMenu ("UI/GamePad Navigation/Selection Group Element")]
	/// <summary>
	/// SelectionGroup Element.
	/// </summary>
	public class SelectionGroupElement : MonoBehaviour, ICancelHandler, IMoveHandler, ISubmitHandler, ISelectHandler, IPointerClickHandler
	{
		[SerializeField] bool _autoExitNavigationOverride;
		public bool autoExitNavigationOverride
		{
			get { return _autoExitNavigationOverride; }
			set
			{
				_autoExitNavigationOverride = value;

				_autoExitNavigationMode = autoExitNavigationOverride ? autoExitNavigationMode : Group.autoExitNavigationMode;
				_exitLeft = autoExitNavigationOverride ? exitLeft : Group.exitLeft;
				_exitRight = autoExitNavigationOverride ? exitRight : Group.exitRight;
				_exitUp = autoExitNavigationOverride ? exitUp : Group.exitUp;
				_exitDown = autoExitNavigationOverride ? exitDown : Group.exitDown;
			}
		}

		[Tooltip ("Exit Sub Selection when edge is reached.\n" +
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

		Navigation.Mode _autoExitNavigationMode;
		bool _exitLeft;
		bool _exitRight;
		bool _exitUp;
		bool _exitDown;

		private Selectable _selectable;

		private SelectionGroup _group;
		public SelectionGroup Group
		{
			get
			{
				#if UNITY_EDITOR
				if (_group == null)
					_group = GetComponentInParent<SelectionGroup>();
				if (_group == null)
					Debug.LogError ("Orphan Sub Selector Component found.", this.gameObject);
				#endif
				return _group;
			}
			set
			{
				_group = value;
				autoExitNavigationOverride = _autoExitNavigationOverride; // forcing initialisation
				enabled = _group != null;
			}
		}

		private SelectionScrollViewElement _thisSelectionScrollViewElement;
		public SelectionScrollViewElement ThisSelectionScrollViewElement
		{
			get
			{
				if (_thisSelectionScrollViewElement == null)
					_thisSelectionScrollViewElement = GetComponent<SelectionScrollViewElement>();
				
				return _thisSelectionScrollViewElement;
			}
		}

		private void Awake ()
		{
//			autoExitNavigationOverride = _autoExitNavigationOverride; // forcing initialisation // FIXED : moved to Group.Set to avoid awake call before Group.Set

			_selectable = GetComponent<Selectable>();

			this.hideFlags = HideFlags.DontSaveInEditor;

//			if (Group) // REMOVED TO FIX DISABLED GROUP ELEMENTS ISSUE
//				enabled = false;
		}

		public void OnCancel (BaseEventData data)
		{
			Group.Exit (_selectable);
		}

		public void OnSubmit (BaseEventData data)
		{
			if (_selectable.interactable)
				Group.SubmitSelection (_selectable);
		}

		public void OnSelect (BaseEventData data)
		{
			if (_selectable.interactable)
				Group.Select (_selectable);
		}

		public void OnPointerClick (PointerEventData data)
		{
			// Submit if Group is already selected and Selectable is interactable
			// Enter Group if Group is not selected
			if (Group.SelectedState && _selectable.interactable)
				Group.SubmitSelection (_selectable);
//			else if (!Group.SelectedState) // FIXED : exit button re-entering
//				Group.Enter (true, true);
		}

		public void OnMove (AxisEventData data)
		{
			if (_selectable.FindSelectable (data.moveVector) != null)
				return;
			
			switch (_autoExitNavigationMode)
			{
				case Navigation.Mode.None: // never exit
					return;
				case Navigation.Mode.Horizontal: // exit if moving left/right and no selectable is found
					switch (data.moveDir)
					{
						case MoveDirection.Up:
						case MoveDirection.Down:
							return;
						default:
							break;
					}
					break;
				case Navigation.Mode.Vertical: // exit if moving up/down and no selectable is found
					switch (data.moveDir)
					{
						case MoveDirection.Left:
						case MoveDirection.Right:
							return;
						default:
							break;
					}
					break;
				case Navigation.Mode.Explicit:
					switch (data.moveDir)
					{
						case MoveDirection.Left:
							if (!_exitLeft)
								return;
							break;
						case MoveDirection.Right:
							if (!_exitRight)
								return;
							break;
						case MoveDirection.Up:
							if (!_exitUp)
								return;
							break;
						case MoveDirection.Down:
							if (!_exitDown)
								return;
							break;
						default:
							return;
					}
					break;
				case Navigation.Mode.Automatic:
					break;
				default:
					return;
			}
			Group.Exit (_selectable, data.moveDir);
		}
	}
}