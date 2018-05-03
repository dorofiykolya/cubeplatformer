//
//  SelectableEvents.cs
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
using UnityEngine.Events;

namespace UnityCoach.GamePadNavigation
{
	[HelpURL ("http://unitycoach.ca/gamepad_navigation")]
	[AddComponentMenu ("UI/GamePad Navigation/Selectable Events")]
	[DisallowMultipleComponent]
	[RequireComponent (typeof (Selectable))]
	/// <summary>
	/// Selectable Events.
	/// Catches Navigation Events and Invoke Unity Events.
	/// </summary>
	public class SelectableEvents : UIBehaviour, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
	{
		[System.Serializable]
		public class MoveDirEvent : UnityEvent <MoveDirection> {}
		[System.Serializable]
		public class MoveVectorEvent : UnityEvent <Vector2> {}
		[System.Serializable]
		public class AxisDataEvent : UnityEvent <AxisEventData> {}

		[SerializeField] UnityEvent onSelect;
		[SerializeField] UnityEvent onDeselect;
		[SerializeField] MoveDirEvent onMoveDir;
		[SerializeField] MoveVectorEvent onMoveVector;
		[SerializeField] AxisDataEvent onMoveData;
		[SerializeField] UnityEvent onSubmit;
		[SerializeField] UnityEvent onCancel;

		public void OnSelect (BaseEventData data)
		{
			onSelect.Invoke ();
		}

		public void OnDeselect (BaseEventData data)
		{
			onDeselect.Invoke ();
		}

		public void OnMove (AxisEventData data) 
		{
			onMoveDir.Invoke (data.moveDir);
			onMoveVector.Invoke (data.moveVector);
			onMoveData.Invoke (data);
		}

		public void OnSubmit (BaseEventData data)
		{
			onSubmit.Invoke ();
		}

		public void OnCancel (BaseEventData data)
		{
			onCancel.Invoke ();
		}
	}
}