//
//  SelectableKeycode.cs
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
	[AddComponentMenu ("UI/GamePad Navigation/Selectable Keycode")]
	/// <summary>
	/// Selectable Keycode.
	/// Executes Selectable Submit Event when a Key is pressed.
	/// </summary>
	public class SelectableKeycode : UIBehaviour
	{
		[Tooltip ("The Keycode associated with this Selectable.")]
		[SerializeField] KeyCode keycode;

		public enum Mode {SelectAndSubmit, SelectOnly, SubmitOnly};
		[SerializeField] Mode mode;

		private Selectable _selectable;

		protected override void Awake ()
		{
			_selectable = GetComponent<Selectable>();
		}
		
		private void Update ()
		{
			if (Input.GetKeyDown(keycode) && _selectable.interactable)
			{
				if (mode == Mode.SelectAndSubmit || mode == Mode.SelectOnly)
					_selectable.Select ();

				if (mode == Mode.SelectAndSubmit || mode == Mode.SubmitOnly)
				{
					BaseEventData data = new BaseEventData (EventSystem.current);
					ExecuteEvents.Execute (gameObject, data, ExecuteEvents.submitHandler);
				}
			}
		}
	}
}