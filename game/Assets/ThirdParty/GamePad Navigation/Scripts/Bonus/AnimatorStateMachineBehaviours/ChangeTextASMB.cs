//
//  ChangeTextASMB.cs
//
//  Author:
//       Frederic Moreau <unity@jikkou.ca>
//
//  Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCoach.UI
{
	[HelpURL ("http://unitycoach.ca/animator_state_machine_behaviours")]
	[AddComponentMenu ("UnityCoach/Animator/StateMachineBehaviours/ChangeText")]
	public class ChangeTextASMB : StateMachineBehaviour
	{
		[SerializeField] string text;
		[SerializeField] bool changeBackOnExit = true;

		string _originalText;
		Text _text;
		bool byPass;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_text = animator.GetComponentInChildren<Text>();
			if (!_text)
			{
				byPass = true;
				return;
			}
			_originalText = _text.text;
			_text.text = text;
		}

		override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			
		}

		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (byPass)
				return;

			if (changeBackOnExit)
				_text.text = _originalText;
		}
	}
}