//
//  ScrollViewAnimatorController.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
//using UnityEditor.UI;
//using UnityEditor.AnimatedValues;

namespace UnityCoach.GamePadNavigation.Editor
{
	static public class ScrollViewAnimatorController
	{
		static public AnimatorController GenerateAnimatorController (string selectionChangedTrigger, string selectedBool, string selectionInt, string selectionSubmittedTrigger, string selectionCancelledTrigger)
		{
			// query save path
			string path = EditorUtility.SaveFilePanelInProject("ScrollView Animator Controller", "ScrollViewController", "controller", "New ScrollView Animator Controller");
			if (path.Length == 0)
				return null;

			// creating new animator controller
			AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath (path);

			controller.AddParameter(selectionChangedTrigger, AnimatorControllerParameterType.Trigger);
			controller.AddParameter(selectedBool, AnimatorControllerParameterType.Bool);
			controller.AddParameter(selectionInt, AnimatorControllerParameterType.Int);
			controller.AddParameter(selectionSubmittedTrigger, AnimatorControllerParameterType.Trigger);
			controller.AddParameter(selectionCancelledTrigger, AnimatorControllerParameterType.Trigger);

			// getting the controller base layer
			AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
			// adding sub state machine
			AnimatorStateMachine selectedStateMachine = controller.layers[0].stateMachine.AddStateMachine ("Selected", new Vector3 (500, 100, 0));

			// adding submit/cancel layer
			controller.AddLayer ("Submit/Cancel");
			AnimatorControllerLayer[] layers = controller.layers;
			layers[1].defaultWeight = 1f;
			controller.layers = layers;

			AnimatorStateMachine submitCancelStateMachine = controller.layers[1].stateMachine;

			// STATES
			AnimatorState defaultState = rootStateMachine.AddState("Default State", new Vector3 (300, 200));
			AnimatorState deselectedState = rootStateMachine.AddState("Deselected", new Vector3 (300, 0, 0));

			AnimatorState selectedState = selectedStateMachine.AddState("Selected", new Vector3 (300, 200, 0));
			AnimatorState selectionChangedState = selectedStateMachine.AddState("Selection Changed", new Vector3 (300, 0, 0));

			AnimatorState defaultSubmitCancelState = submitCancelStateMachine.AddState("Default State", new Vector3 (410, 200));
			AnimatorState submitState = submitCancelStateMachine.AddState("Submit", new Vector3 (300, 0, 0));
			AnimatorState cancelState = submitCancelStateMachine.AddState("Cancel", new Vector3 (520, 0, 0));
			submitCancelStateMachine.anyStatePosition = new Vector3 (420, -100, 0);

			// TRANSITIONS
			// Default to Selected
			AnimatorStateTransition selectedTransition = defaultState.AddTransition(selectedStateMachine);
			selectedTransition.AddCondition (AnimatorConditionMode.If, 0, selectedBool);

			// Deselected to Default
			deselectedState.AddTransition(defaultState, true);

			// Selected 'Any' to SelectionChanged
			AnimatorStateTransition selectionChangedTransition = rootStateMachine.AddAnyStateTransition(selectionChangedState);
			selectionChangedTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, selectionChangedTrigger);

			// SelectionChanged back to Selected
			selectionChangedState.AddTransition (selectedState, true);

			// Selected to Exit
			AnimatorStateTransition exitTransition = selectedState.AddExitTransition();
			exitTransition.AddCondition(AnimatorConditionMode.IfNot, 0, selectedBool);

			// Selected 'Exit' to Deselect
			rootStateMachine.AddStateMachineTransition(selectedStateMachine, deselectedState);

			// Default 'Any' to Submit
			AnimatorStateTransition selectionSubmitTransition = submitCancelStateMachine.AddAnyStateTransition(submitState);
			selectionSubmitTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, selectionSubmittedTrigger);

			// Submit back to Default
			submitState.AddTransition (defaultSubmitCancelState, true);

			// Default 'Any' to Cancel
			AnimatorStateTransition selectionCancelTransition = submitCancelStateMachine.AddAnyStateTransition(cancelState);
			selectionCancelTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, selectionCancelledTrigger);

			// Cancel back to Default
			cancelState.AddTransition (defaultSubmitCancelState, true);

			// creating the animation clips
			AnimationClipSettings clipSettings = new AnimationClipSettings ();
			clipSettings.loopTime = false;
			AnimationClip defaultAnimationClip = new AnimationClip();
			defaultAnimationClip.name = "Default";
			AnimationUtility.SetAnimationClipSettings(defaultAnimationClip, clipSettings);

			AnimationClip selectedAnimationClip = new AnimationClip();
			selectedAnimationClip.name = "ListSelected";
			AnimationUtility.SetAnimationClipSettings(selectedAnimationClip, clipSettings);

			AnimationClip deselectedAnimationClip = new AnimationClip();
			deselectedAnimationClip.name = "ListDeselected";
			AnimationUtility.SetAnimationClipSettings(deselectedAnimationClip, clipSettings);

			AnimationClip selectionChangedAnimationClip = new AnimationClip();
			selectionChangedAnimationClip.name = "SelectionChanged";
			AnimationUtility.SetAnimationClipSettings(selectionChangedAnimationClip, clipSettings);

			AnimationClip selectionSubmittedAnimationClip = new AnimationClip();
			selectionSubmittedAnimationClip.name = "SelectionSubmitted";
			AnimationUtility.SetAnimationClipSettings(selectionSubmittedAnimationClip, clipSettings);

			AnimationClip selectionCancelledAnimationClip = new AnimationClip();
			selectionCancelledAnimationClip.name = "SelectionCancelled";
			AnimationUtility.SetAnimationClipSettings(selectionCancelledAnimationClip, clipSettings);

			// assigning state motion clips
			defaultState.motion = defaultAnimationClip;
			selectedState.motion = selectedAnimationClip;
			deselectedState.motion = deselectedAnimationClip;
			selectionChangedState.motion = selectionChangedAnimationClip;
			submitState.motion = selectionSubmittedAnimationClip;
			cancelState.motion = selectionCancelledAnimationClip;

			// bundling the animation clips with the controller
			AssetDatabase.AddObjectToAsset(defaultAnimationClip, controller);
			AssetDatabase.AddObjectToAsset(selectedAnimationClip, controller);
			AssetDatabase.AddObjectToAsset(deselectedAnimationClip, controller);
			AssetDatabase.AddObjectToAsset(selectionChangedAnimationClip, controller);
			AssetDatabase.AddObjectToAsset(selectionSubmittedAnimationClip, controller);
			AssetDatabase.AddObjectToAsset(selectionCancelledAnimationClip, controller);

			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(selectedAnimationClip));

			return controller;
		}
	}
}
