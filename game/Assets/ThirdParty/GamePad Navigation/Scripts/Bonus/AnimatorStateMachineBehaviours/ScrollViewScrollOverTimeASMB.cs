//
//  ScrollViewScrollOverTimeASMB.cs
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
	[AddComponentMenu ("UnityCoach/Animator/StateMachineBehaviours/ScrollViewScrollOverTime")]
	public class ScrollViewScrollOverTimeASMB : StateMachineBehaviour
	{
		[Tooltip ("Curve to control scrolling time.")]
		[SerializeField] AnimationCurve navigationScrollingCurve = new AnimationCurve (new Keyframe [2] {new Keyframe(0,0), new Keyframe(1, 1)});

		ScrollRect _scrollRect;
		bool byPass;

		private float t;
		private float _lCurve;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_scrollRect = animator.GetComponent<ScrollRect>();
			if (!_scrollRect)
				byPass = true;
		}
			
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (byPass)
				return;
			
			t = stateInfo.normalizedTime - Mathf.Floor(stateInfo.normalizedTime);
			_lCurve = navigationScrollingCurve.Evaluate (t);
			_scrollRect.normalizedPosition = Vector2.Lerp(Vector2.zero, Vector2.one, _lCurve);
		}
			
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (byPass)
				return;
			
			_scrollRect.normalizedPosition = Vector2.zero;
		}
	}
}