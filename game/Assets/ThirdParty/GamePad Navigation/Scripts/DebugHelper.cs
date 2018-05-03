//
//  DebugHelper.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[DisallowMultipleComponent]
	[AddComponentMenu ("")] // hidden
	/// <summary>
	/// Debug helper for Selection Scroll Views and Selection Groups.
	/// </summary>
	public class DebugHelper : MonoBehaviour
	{
		[SerializeField] SelectionScrollView _sScrollView;
		[SerializeField] SelectionGroup _sGroup;

		void Group_SelectionExit ()
		{
			Debug.Log (string.Format ("Selection Group {0} Exited", _sGroup.gameObject.name), _sGroup);
		}

		void Group_SelectionSubmitted (int selection)
		{
			Debug.Log (string.Format ("Selection Group {0} Submitted Selection {1}", _sGroup.gameObject.name, selection), _sGroup);
		}

		void Group_SelectionChanged (int selection)
		{
			Debug.Log (string.Format ("Selection Group {0} Changed Selection {1}", _sGroup.gameObject.name, selection), _sGroup);
		}

		void Group_SelectionEnter ()
		{
			Debug.Log (string.Format ("Selection Group {0} Entered", _sGroup.gameObject.name), _sGroup);
		}

		void Group_Initialised ()
		{
			Debug.Log (string.Format ("Selection Group {0} Initialised", _sGroup.gameObject.name), _sGroup);
		}

		void SSV_SelectionCancelled (int selection)
		{
			Debug.Log (string.Format ("Selection ScrollView {0} Cancelled Selection {1}", _sScrollView.gameObject.name, selection), _sScrollView);
		}

		void SSV_SelectionSubmitted (int selection)
		{
			Debug.Log (string.Format ("Selection ScrollView {0} Submitted Selection {1}", _sScrollView.gameObject.name, selection), _sScrollView);
		}

		void SSV_SelectionChanged (int selection)
		{
			Debug.Log (string.Format ("Selection ScrollView {0} Changed Selection {1}", _sScrollView.gameObject.name, selection), _sScrollView);
		}

		void SSV_SelectedChanged (bool selected)
		{
			Debug.Log (string.Format ("Selection ScrollView {0} {1}", _sScrollView.gameObject.name, selected ? "Selected" : "Deselected"), _sScrollView);
		}

		void SSV_ViewInitialised ()
		{
			Debug.Log (string.Format ("Selection ScrollView {0} Initialised", _sScrollView.gameObject.name), _sScrollView);
		}

		void Start ()
		{
			if (_sScrollView)
			{
				_sScrollView.ViewInitialised += SSV_ViewInitialised;
				_sScrollView.SelectedChanged += SSV_SelectedChanged;
				_sScrollView.SelectionChanged += SSV_SelectionChanged;
				_sScrollView.SelectionSubmitted += SSV_SelectionSubmitted;
				_sScrollView.SelectionCancelled += SSV_SelectionCancelled;
			}

			if (_sGroup)
			{
				_sGroup.SelectorInitialised += Group_Initialised;
				_sGroup.SelectionEnter += Group_SelectionEnter;
				_sGroup.SelectionChanged += Group_SelectionChanged;
				_sGroup.SelectionSubmitted += Group_SelectionSubmitted;
				_sGroup.SelectionExit += Group_SelectionExit;
			}
		}

		void OnDestroy ()
		{
			if (_sScrollView)
			{
				_sScrollView.ViewInitialised -= SSV_ViewInitialised;
				_sScrollView.SelectedChanged -= SSV_SelectedChanged;
				_sScrollView.SelectionChanged -= SSV_SelectionChanged;
				_sScrollView.SelectionSubmitted -= SSV_SelectionSubmitted;
				_sScrollView.SelectionCancelled -= SSV_SelectionCancelled;
			}

			if (_sGroup)
			{
				_sGroup.SelectorInitialised -= Group_Initialised;
				_sGroup.SelectionEnter -= Group_SelectionEnter;
				_sGroup.SelectionChanged -= Group_SelectionChanged;
				_sGroup.SelectionSubmitted -= Group_SelectionSubmitted;
				_sGroup.SelectionExit -= Group_SelectionExit;
			}
		}
	}
}