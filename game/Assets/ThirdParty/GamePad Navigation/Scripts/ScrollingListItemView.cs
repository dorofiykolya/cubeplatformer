//
//  ScrollingListItemView.cs
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
	[DisallowMultipleComponent]
	[SelectionBase]
	[AddComponentMenu ("UI/GamePad Navigation/Scrolling List Item")]
	/// <summary>
	/// Navigable Scroll View element.
	/// </summary>
	public class ScrollingListItemView : UIBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
	{
		#region Selectable Members

		public enum SelectionState { Normal, Highlighted, Pressed, Disabled };
		public enum Transition { None, ColorTint, SpriteSwap, Animation };

		public Transition transition = Transition.ColorTint;
		public Graphic targetGraphic;
		public ColorBlock colors = ColorBlock.defaultColorBlock;
		public SpriteState spriteState;

		private SelectionState _currentSelectionState;

		public Image image
		{
			get { return targetGraphic as Image; }
			set { targetGraphic = value; }
		}

		#endregion

		#region Animator
		private Animator _animator;

		[Header ("Animator Parameters")]
		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when the Element is not highlighted.")]
		public string normalTrigger = "Normal";
		private int _normalHashID;

		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when the Element is highlighted.")]
		public string highlightedTrigger = "Highlighted";
		private int _highlightedHashID;

		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when the Element is pressed.")]
		public string pressedTrigger = "Pressed";
		private int _pressedHashID;

		[Tooltip ("The name of the Animator Trigger Parameter to be triggered when the Element is disabled.")]
		public string disabledTrigger = "Disabled";
		private int _disabledHashID;
		#endregion

		public UnityEvent onSelect;
		public UnityEvent onDeselect;
		public UnityEvent onSubmit;

		private ScrollingListView _ssview;
		private SelectionState _stateWhenDisabled = SelectionState.Normal;

		protected override void Awake ()
		{
			base.Awake ();
			_ssview = GetComponentInParent<ScrollingListView>();
			if (!_ssview)
				enabled = false;

			_animator = GetComponent<Animator>();

			_normalHashID = Animator.StringToHash(normalTrigger);
			_highlightedHashID = Animator.StringToHash(highlightedTrigger);
			_pressedHashID = Animator.StringToHash(pressedTrigger);
			_disabledHashID = Animator.StringToHash(disabledTrigger);
		}

		public void OnSelect (BaseEventData data)
		{
			onSelect.Invoke ();

			DoStateTransition(SelectionState.Highlighted, false);
		}

		public void OnDeselect(BaseEventData data)
		{
			onDeselect.Invoke ();
			StopAllCoroutines();
			DoStateTransition(SelectionState.Normal, false);
		}

		public void OnSubmit (BaseEventData data)
		{
			onSubmit.Invoke ();

			DoStateTransition(SelectionState.Pressed, false, true);
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();
			DoStateTransition(_stateWhenDisabled, true);
		}

		protected override void OnDisable ()
		{
			_stateWhenDisabled = currentSelectionState;
			base.OnDisable ();
			DoStateTransition(SelectionState.Disabled, false);
		}

		#region Selectable Methods
		protected SelectionState currentSelectionState
		{
			get { return _currentSelectionState; }
		}

		protected virtual void InstantClearState()
		{
			int hashID = _normalHashID;

			switch (transition)
			{
				case Transition.ColorTint:
					StartColorTween(Color.white, true);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(null);
					break;
				case Transition.Animation:
					TriggerAnimation(hashID);
					break;
			}
		}

		protected virtual void DoStateTransition(SelectionState state, bool instant, bool back = false)
		{
			Color tintColor;
			Sprite transitionSprite;
			int hashID;

			SelectionState currentState = currentSelectionState;

			_currentSelectionState = state;

			switch (state)
			{
				case SelectionState.Normal:
					tintColor = colors.normalColor;
					transitionSprite = null;
					hashID = _normalHashID;
					break;
				case SelectionState.Highlighted:
					tintColor = colors.highlightedColor;
					transitionSprite = spriteState.highlightedSprite;
					hashID = _highlightedHashID;
					break;
				case SelectionState.Pressed:
					tintColor = colors.pressedColor;
					transitionSprite = spriteState.pressedSprite;
					hashID = _pressedHashID;
					break;
				case SelectionState.Disabled:
					tintColor = colors.disabledColor;
					transitionSprite = spriteState.disabledSprite;
					hashID = _disabledHashID;
					break;
				default:
					tintColor = Color.black;
					transitionSprite = null;
					hashID = 0;
					break;
			}

			if (gameObject.activeInHierarchy)
			{
				switch (transition)
				{
					case Transition.ColorTint:
						StartColorTween(tintColor * colors.colorMultiplier, instant);
						break;
					case Transition.SpriteSwap:
						DoSpriteSwap(transitionSprite);
						break;
					case Transition.Animation:
						TriggerAnimation(hashID);
						break;
				}

				if (back)
					StartCoroutine (TransitionBack (currentState));
			}
		}

		private IEnumerator TransitionBack (SelectionState state, bool instant = false)
		{
			yield return new WaitForSeconds(colors.fadeDuration);
			DoStateTransition(state, instant);
		}

		private void StartColorTween(Color targetColor, bool instant)
		{
			if (targetGraphic == null)
				return;

			targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
		}

		private void DoSpriteSwap(Sprite newSprite)
		{
			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		private void TriggerAnimation(int triggerHashID)
		{
			if (_animator == null || !_animator.isActiveAndEnabled || _animator.runtimeAnimatorController == null || triggerHashID == 0)
				return;

			_animator.ResetTrigger(_normalHashID);
			_animator.ResetTrigger(_pressedHashID);
			_animator.ResetTrigger(_highlightedHashID);
			_animator.ResetTrigger(_disabledHashID);
			_animator.SetTrigger(triggerHashID);
		}

		#if UNITY_EDITOR
		protected override void Reset ()
		{
			base.Reset ();
			targetGraphic = GetComponent<Graphic>();
		}

		public static void ConvertSelectableToNSVE (Selectable selectable)
		{
			GameObject obj = selectable.gameObject;

			ScrollingListItemView ssve = UnityEditor.Undo.AddComponent<ScrollingListItemView>(obj.gameObject);

			UnityEditor.Undo.RecordObject (ssve, "ssve");

			ssve.colors = selectable.colors;
			ssve.spriteState = selectable.spriteState;
			ssve.targetGraphic = selectable.targetGraphic;
			ssve.normalTrigger = selectable.animationTriggers.normalTrigger;
			ssve.highlightedTrigger = selectable.animationTriggers.highlightedTrigger;
			ssve.pressedTrigger = selectable.animationTriggers.pressedTrigger;
			ssve.disabledTrigger = selectable.animationTriggers.disabledTrigger;
			ssve.transition = (ScrollingListItemView.Transition) selectable.transition;

			UnityEditor.Undo.DestroyObjectImmediate (selectable);
		}
		#endif
		#endregion
	}
}