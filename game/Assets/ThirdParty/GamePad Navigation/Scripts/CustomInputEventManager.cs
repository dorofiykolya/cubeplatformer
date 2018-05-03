//
//  CustomInputEventManager.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * TODO : Add support for GetKeyUp / GetButtonUp and GetKey / GetButton
 */

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[AddComponentMenu ("UI/GamePad Navigation/Custom Input Event Manager")]
	[DisallowMultipleComponent]
	/// <summary>
	/// Custom Input Event Manager.
	/// Handles custom inputs (buttons and keys) on current event system selected object.
	/// </summary>
	public class CustomInputEventManager : MonoBehaviour
	{
		[SerializeField] string [] _customInputs;
		[SerializeField] KeyCode [] _customKeycodes;

		public static string [] customInputs;
		public static KeyCode [] customKeycodes;

		/// <summary>
		/// The custom input handler.
		/// </summary>
		public static ICustomInputHandler customInputHandler;
		/// <summary>
		/// The custom keycode handler.
		/// </summary>
		public static ICustomKeycodeHandler customKeycodeHandler;

		public delegate void SelectionChange (GameObject selection);
		/// <summary>
		/// Occurs when selection changed.
		/// </summary>
		public static event SelectionChange SelectionChanged;

		static GameObject _currentSelectedGameObject;
		static GameObject currentSelectedGameObject
		{
			get { return _currentSelectedGameObject; }
			set
			{
				if (_currentSelectedGameObject != value)
				{
					_currentSelectedGameObject = value;
					if (_currentSelectedGameObject != null)
					{
						customInputHandler = _currentSelectedGameObject.GetComponent<ICustomInputHandler>();
						customKeycodeHandler = _currentSelectedGameObject.GetComponent<ICustomKeycodeHandler>();
					}

					if (SelectionChanged != null)
						SelectionChanged (_currentSelectedGameObject);
				}
			}
		}

		void Start ()
		{
			customInputs = _customInputs;
			customKeycodes = _customKeycodes;

			if (_customInputs.Length == 0 && _customKeycodes.Length == 0)
				enabled = false;
		}

		void Update ()
		{
			currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

			if (currentSelectedGameObject == null)
				return;

			if (customInputHandler != null)
			{
				for (int i = 0 ; i < _customInputs.Length ; i++)
				{
					if (Input.GetButtonDown (_customInputs[i]) && customInputHandler.IsHandlingInput(_customInputs[i]))
						customInputHandler.OnCustomInput (_customInputs[i]);
				}
			}

			if (customKeycodeHandler != null)
			{
				for (int i = 0 ; i < _customKeycodes.Length ; i++)
				{
					if (Input.GetKeyDown (_customKeycodes[i]) && customKeycodeHandler.IsHandlingKeycode(_customKeycodes[i]))
						customKeycodeHandler.OnCustomKeycode (_customKeycodes[i]);
				}
			}			
		}
	}
}