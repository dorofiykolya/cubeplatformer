//
//  ScrollingListItemViewEditor.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityCoach.GamePadNavigation;

namespace UnityCoach.GamePadNavigation.Editor
{
	[CustomEditor(typeof(ScrollingListItemView))]
	[CanEditMultipleObjects]
	public class ScrollingListItemViewEditor : UnityEditor.Editor
	{
		#region Transition
		SerializedProperty transition;
		SerializedProperty targetGraphic;
		SerializedProperty colors;
		SerializedProperty spriteState;
		#endregion

		#region Animator
		SerializedProperty normalTrigger;
		SerializedProperty highlightedTrigger;
		SerializedProperty pressedTrigger;
		SerializedProperty disabledTrigger;
		#endregion

		#region Unity Events
		SerializedProperty onSelect;
		SerializedProperty onDeselect;
		SerializedProperty onSubmit;
		#endregion

		protected void OnEnable()
		{
			transition = serializedObject.FindProperty("transition");
			targetGraphic = serializedObject.FindProperty("targetGraphic");
			colors = serializedObject.FindProperty("colors");
			spriteState = serializedObject.FindProperty("spriteState");

			normalTrigger = serializedObject.FindProperty("normalTrigger");
			highlightedTrigger = serializedObject.FindProperty("highlightedTrigger");
			pressedTrigger = serializedObject.FindProperty("pressedTrigger");
			disabledTrigger = serializedObject.FindProperty("disabledTrigger");

			onSelect = serializedObject.FindProperty("onSelect");
			onDeselect = serializedObject.FindProperty("onDeselect");
			onSubmit = serializedObject.FindProperty("onSubmit");
		}

		bool _displayInfo;
		string _info = "Navigable ScrollView Element Component\n" +
			"Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.\n" +
			"Twitter : @UnityCoach\n" +
			"More information on http://unitycoach.ca";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_displayInfo = EditorGUILayout.Foldout (_displayInfo, "Information");
			if (_displayInfo)
				EditorGUILayout.HelpBox (_info, MessageType.None);

			EditorGUILayout.PropertyField(transition);

			switch (((ScrollingListItemView)(serializedObject.targetObject)).transition)
			{
				case ScrollingListItemView.Transition.ColorTint :
					EditorGUILayout.PropertyField(targetGraphic);
					EditorGUILayout.PropertyField(colors);
					break;
				case ScrollingListItemView.Transition.SpriteSwap :
					EditorGUILayout.PropertyField(targetGraphic);
					EditorGUILayout.PropertyField(spriteState);
					break;
				case ScrollingListItemView.Transition.Animation :
					EditorGUILayout.PropertyField(normalTrigger);
					EditorGUILayout.PropertyField(highlightedTrigger);
					EditorGUILayout.PropertyField(pressedTrigger);
					EditorGUILayout.PropertyField(disabledTrigger);
					// TODO : Add Animator Generation
//					if (this.targets.Length == 1)
//					{
//						if (GUILayout.Button("Generate Animator Controller", EditorStyles.miniButton))
//						{
//							serializedObject.ApplyModifiedProperties();
//							
//							// parameters
//							string normalTrigger = ((SelectableScrollViewElement)this.target).normalTrigger;
//							string highlightedTrigger = ((SelectableScrollViewElement)this.target).highlightedTrigger;
//							string pressedTrigger = ((SelectableScrollViewElement)this.target).pressedTrigger;
//							string disabledTrigger = ((SelectableScrollViewElement)this.target).disabledTrigger;
//							
//							RuntimeAnimatorController controller = ScrollViewAnimatorController.GenerateAnimatorController (selectionChangedTrigger, selectedBool, selectionInt, selectionSubmittedTrigger, selectionCancelledTrigger);;
//							
//							if (controller)
//							{
//								Animator _animator = ((SelectableScrollViewElement)this.target).GetComponent<Animator>();
//								
//								if (!_animator)
//									_animator = ((SelectableScrollViewElement)this.target).gameObject.AddComponent<Animator>();
//								
//								_animator.runtimeAnimatorController = controller;
//							}
//						}
//					}
//					else
//					{
//						EditorGUILayout.HelpBox ("Animator Controller Generation not supported on multiple objects.", MessageType.Info);
//					}
					break;
				default :
					break;
			}

			EditorGUILayout.PropertyField(onSelect);
			EditorGUILayout.PropertyField(onDeselect);
			EditorGUILayout.PropertyField(onSubmit);

			serializedObject.ApplyModifiedProperties();
		}

		#region Static Methods
		[MenuItem ("CONTEXT/Selectable/Convert To Navigable ScrollView Element")]
		static void ConvertToNSVE (MenuCommand command)
		{
			Selectable selectable = (Selectable)command.context;

			int undoGroup = UnityEditor.Undo.GetCurrentGroup();
			Undo.SetCurrentGroupName ("Convert To Navigable ScrollView Element");

			ScrollingListItemView.ConvertSelectableToNSVE (selectable);

			Undo.CollapseUndoOperations (undoGroup);
		}

		[MenuItem ("CONTEXT/Selectable/Convert To Navigable ScrollView Element", true)]
		static bool ConvertToNSVE_Validate (MenuCommand command)
		{
			return (((Selectable)command.context).GetComponentInParent<ScrollingListView>() != null);
		}
		#endregion
	}
}