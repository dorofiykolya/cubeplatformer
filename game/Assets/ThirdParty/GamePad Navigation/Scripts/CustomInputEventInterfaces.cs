//
//  CustomInputEventInterfaces.cs
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
	/// <summary>
	/// ICustomInputHandler
	/// Implement to receive custom Input (Input.GetButtonDown) on a Selectable object
	/// </summary>
	public interface ICustomInputHandler
	{
		bool IsHandlingInput (string input);
		void OnCustomInput (string input);
	}

	/// <summary>
	/// ICustomKeycodeHandler
	/// Implement to receive custom KeyCode (Input.GetKeyDown) on a Selectable object
	/// </summary>
	public interface ICustomKeycodeHandler
	{
		bool IsHandlingKeycode (KeyCode keycode);
		void OnCustomKeycode (KeyCode keycode);
	}
}