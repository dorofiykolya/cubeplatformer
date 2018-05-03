//
//  DisableImageUponStart.cs
//
//  Author:
//       Frederic Moreau <unity@jikkou.ca>
//
//  Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableImageUponStart : MonoBehaviour
{
	void Awake ()
	{
		Image img = GetComponent<Image>();
		if (img)
			img.enabled = false;
	}
}