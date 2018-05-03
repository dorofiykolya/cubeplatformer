//
//  ScrollViewHelper.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

using UnityEngine;
using System.Collections;

namespace UnityCoach.GamePadNavigation
{
	/// <summary>
	/// Scroll Views Static Methods.
	/// </summary>
	static public class ScrollViewHelper
	{
		public enum ScrollingMode {Vertical, Horizontal, Grid};
		public enum Alignment {None, Min, Center, Max, Pivot};

		static public void SetNormalisedPosition (ref Vector2 nPosition, int selection, int count, ScrollingMode mode)
		{
			switch (mode)
			{
				case ScrollingMode.Vertical :
					nPosition.y = 1f - ((float)selection / (count - 1));
					break;
				case ScrollingMode.Horizontal :
					nPosition.x = ((float)selection / (count - 1));
					break;
			}
		}

		static public void SetNormalisedPosition (ref Vector2 nPosition, RectTransform viewport, RectTransform content, RectTransform selection, RectTransform guide, ScrollingMode mode, Alignment alignment)
		{
			float aFrom;
			float aTo;

			switch (mode)
			{
				case ScrollingMode.Vertical :
					switch (alignment)
					{
						case Alignment.Min : 
							aFrom = -selection.offsetMin.y / content.sizeDelta.y;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.min)).y) / content.sizeDelta.y;
							nPosition.y = 1f - (aFrom + aTo);
							break;
						case Alignment.Center : 
							aFrom = (-((selection.offsetMax.y-selection.offsetMin.y)/2)-selection.offsetMin.y) / content.sizeDelta.y;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.center)).y) / content.sizeDelta.y;
							nPosition.y = 1f - (aFrom + aTo);
							break;
						case Alignment.Max :
							aFrom = -selection.offsetMax.y / content.sizeDelta.y;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.max)).y) / content.sizeDelta.y;
							nPosition.y = 1f - (aFrom + aTo);
							break;
						case Alignment.Pivot :
							aFrom = -selection.anchoredPosition.y / content.sizeDelta.y;
							aTo = (viewport.InverseTransformPoint(guide.position).y) / content.sizeDelta.y;
							nPosition.y = 1f - (aFrom + aTo);
							break;
					}
					break;
				case ScrollingMode.Horizontal : 
					switch (alignment)
					{
						case Alignment.Min : 
							aFrom = selection.offsetMin.x / content.sizeDelta.x;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.min)).x) / content.sizeDelta.x;
							nPosition.x = aFrom - aTo;
							break;
						case Alignment.Center : 
							aFrom = (((selection.offsetMax.x-selection.offsetMin.x)/2)+selection.offsetMin.x) / content.sizeDelta.x;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.center)).x) / content.sizeDelta.x;
							nPosition.x = aFrom - aTo;
							break;
						case Alignment.Max :
							aFrom = selection.offsetMax.x / content.sizeDelta.x;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.max)).x) / content.sizeDelta.x;
							nPosition.x = aFrom - aTo;
							break;
						case Alignment.Pivot :
							aFrom = selection.anchoredPosition.x / content.sizeDelta.x;
							aTo = (viewport.InverseTransformPoint(guide.position).x) / content.sizeDelta.x;
							nPosition.x = aFrom - aTo;
							break;
					}
					break;
			}
		}

		static public void SetDeltaPosition (ref Vector2 nPosition, RectTransform viewport, RectTransform selection, RectTransform guide, ScrollingMode mode, Alignment alignment)
		{
			float aFrom;
			float aTo;

			switch (mode)
			{
				case ScrollingMode.Vertical :
					switch (alignment)
					{
						case Alignment.Min : 
							aFrom = selection.offsetMin.y;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.min)).y);
							nPosition.y = aTo - aFrom;
							break;
						case Alignment.Center : 
							aFrom = (((selection.offsetMax.y-selection.offsetMin.y)/2)+selection.offsetMin.y);
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.center)).y);
							nPosition.y = aTo - aFrom;
							break;
						case Alignment.Max :
							aFrom = selection.offsetMax.y;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.max)).y);
							nPosition.y = aTo - aFrom;
							break;
						case Alignment.Pivot :
							aFrom = selection.anchoredPosition.y;
							aTo = (viewport.InverseTransformPoint(guide.position).y);
							nPosition.y = aTo - aFrom;
							break;
					}
					break;
				case ScrollingMode.Horizontal : 
					switch (alignment)
					{
						case Alignment.Min : 
							aFrom = selection.offsetMin.x;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.min)).x);
							nPosition.x = aTo - aFrom;
							break;
						case Alignment.Center : 
							aFrom = (((selection.offsetMax.x-selection.offsetMin.x)/2)+selection.offsetMin.x);
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.center)).x);
							nPosition.x = aTo - aFrom;
							break;
						case Alignment.Max :
							aFrom = selection.offsetMax.x;
							aTo = (viewport.InverseTransformPoint(guide.TransformPoint(guide.rect.max)).x);
							nPosition.x = aTo - aFrom;
							break;
						case Alignment.Pivot :
							aFrom = selection.anchoredPosition.x;
							aTo = (viewport.InverseTransformPoint(guide.position).x);
							nPosition.x = aTo - aFrom;
							break;
					}
					break;
					case ScrollingMode.Grid :
						nPosition = ((Vector2)viewport.InverseTransformPoint(guide.position)) - selection.anchoredPosition;
					break;
			}
		}
	}
}