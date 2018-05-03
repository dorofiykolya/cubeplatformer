//
//  SelectionScrollViewElement.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[RequireComponent (typeof (Selectable))]
	[DisallowMultipleComponent]
	[AddComponentMenu ("UI/GamePad Navigation/Selection ScrollView Element")]
	/// <summary>
	/// Selection Scroll View element.
	/// Pushes events up to the parent Scroll View.
	/// </summary>
	public class SelectionScrollViewElement : UIBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler, IMoveHandler, IPointerClickHandler
	{
		private SelectionScrollView _ssview;
		private Selectable _selectable;

		protected override void Awake ()
		{
			base.Awake ();
			_ssview = GetComponentInParent<SelectionScrollView>();
			if (!_ssview)
				enabled = false;
			_selectable = GetComponent<Selectable>();
		}

		public void OnSelect (BaseEventData data)
		{
			if (_selectable.interactable)
				_ssview.Select (this);
		}

		public void OnMove (AxisEventData data)
		{
			Selectable s = _selectable.FindSelectable (data.moveVector);
			SelectionScrollViewElement e = s!=null ? s.GetComponent<SelectionScrollViewElement>() : null;

			// deselect if target exists AND target has element AND element isnt in list
			if (s && !e && !_ssview.Contains(e))
				_ssview.Deselect (this);			
		}

		public void OnSubmit (BaseEventData data)
		{
			if (_selectable.interactable)
				_ssview.Submit();
		}

		public void OnCancel (BaseEventData data)
		{
			_ssview.Cancel ();
		}

		public void OnPointerClick (PointerEventData data)
		{
			if (_selectable.interactable)
			{
				_ssview.Select (this);
				_ssview.Submit();
			}
		}
	}
}