//
//  DisableBehaviourUponAwake.cs
//
//  Author:
//       Frederic Moreau <unity@jikkou.ca>
//
//  Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.

using UnityEngine;

public class DisableBehaviourUponAwake : MonoBehaviour
{
	[SerializeField] Behaviour behaviour;

	void Awake ()
	{
		if (behaviour)
			behaviour.enabled = false;
	}
}