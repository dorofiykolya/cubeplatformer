//
//  GradientImage.cs
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
	[HelpURL ("http://unitycoach.ca/ui")]
	[AddComponentMenu ("UI/UnityCoach/Gradient Image")]
	[DisallowMultipleComponent]
	[RequireComponent (typeof (RawImage))]
	/// <summary>
	/// Gradient Image.
	/// Generates a gradient image and pushes it into a Raw Image texture.
	/// </summary>
	public class GradientImage : MonoBehaviour
	{
		public enum Type {Horizontal, Vertical, Radial};

		[SerializeField] Gradient _gradient;
		[SerializeField] Type _type;
		[SerializeField] Vector2 _radialCenter;
		[SerializeField] [Range (0.01f, 10f)] float _radialSize = 1f;
		RawImage _rawImage;
		[SerializeField] [Range (1, 2048)] int _width = 32;
		[SerializeField] [Range (1, 2048)] int _height = 32;
		[SerializeField] FilterMode _filterMode = FilterMode.Bilinear;
		[SerializeField] TextureWrapMode _wrapMode = TextureWrapMode.Clamp;

		public Gradient gradient
		{
			get { return _gradient; }
			set
			{
				_gradient = value;
				GenerateGradient (_width, _height);
			}
		}

		void Awake ()
		{
			_rawImage = GetComponent<RawImage>();
		}

		void Start ()
		{
			GenerateGradient (_width, _height);
		}

		void GenerateGradient (int width = 64, int height = 64)
		{
			int w = width, h = height;
//			float ratio = (float)w/(float)h;

			if (_type == Type.Horizontal)
				h = 1;
			else if (_type == Type.Vertical)
				w = 1;

			Texture2D tx = new Texture2D (w, h);

			switch (_type)
			{
				case Type.Horizontal :
					for (int col = 0 ; col < w ; col++)
						for (int row = 0 ; row < h ; row++)
							tx.SetPixel (col, row, _gradient.Evaluate ((float)col / (float)w));
					break;
				case Type.Vertical :
					for (int col = 0 ; col < w ; col++)
						for (int row = 0 ; row < h ; row++)
							tx.SetPixel (col, row, _gradient.Evaluate ((float)row / (float)h));
					break;
				case Type.Radial :
					int shorterEdge = Mathf.Min(w, h);
					Vector2 center = _radialCenter;
					center.Scale (new Vector2 (w, h));
					center /= shorterEdge;

					for (int col = 0 ; col < w ; col++)
					{
						for (int row = 0 ; row < h ; row++)
						{
							float distance = Vector2.Distance ( center, new Vector2 ((float)col / (float)shorterEdge, (float)row / (float)shorterEdge) );
							tx.SetPixel (col, row, _gradient.Evaluate (distance * 2f/_radialSize));
						}
					}
					break;
			}
			
			tx.filterMode = _filterMode;
			tx.wrapMode = _wrapMode;
			tx.Apply ();
			_rawImage.texture = tx;
		}

		void OnValidate ()
		{
			if (!_rawImage)
				_rawImage = GetComponent<RawImage>();

			GenerateGradient (_width, _height);
		}

		void Reset ()
		{
			_rawImage = GetComponent<RawImage>();
			Vector2 size = ((RectTransform)(_rawImage.transform)).rect.size;
			_width = (int)size.x/10;
			_height = (int)size.y/10;
		}
	}
}