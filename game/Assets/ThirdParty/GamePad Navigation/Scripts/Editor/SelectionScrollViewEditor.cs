//
//  SelectionScrollViewEditor.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
//using UnityEditor.Animations;
using UnityCoach.GamePadNavigation;

namespace UnityCoach.GamePadNavigation.Editor
{
	[CustomEditor(typeof(SelectionScrollView))]
	[CanEditMultipleObjects]
	public class SelectionScrollViewEditor : UnityEditor.Editor
	{
		SerializedProperty scrollingMode;

		#region Alignment
		SerializedProperty alignment;
		SerializedProperty alignmentGuide;
		SerializedProperty forceScrolling;
		SerializedProperty updateTransforms;
		SerializedProperty alwaysUpdate;
		#endregion

		#region Animation
		SerializedProperty navigationScrollingTime;
		SerializedProperty navigationScrollingCurve;
		SerializedProperty initOnStart;
		SerializedProperty autoAddElementComponentToSelectablesOnInit;
		#endregion

		#region Animator
		SerializedProperty selectionChangedTrigger;
		SerializedProperty selectedBool;
		SerializedProperty selectionInt;
		SerializedProperty selectionSubmittedTrigger;
		SerializedProperty selectionCancelledTrigger;
		#endregion

		#region Unity Events
		SerializedProperty silentOnInitialSelection;
		SerializedProperty onSelectionChanged;
		SerializedProperty onListSelected;
		SerializedProperty onListDeselected;
		SerializedProperty onSubmit;
		SerializedProperty onCancel;
		SerializedProperty onListSelectedAsBoolean;
		SerializedProperty onListDeselectedAsBoolean;
		#endregion

		Texture headerTexture;

		protected void OnEnable()
		{
			scrollingMode = serializedObject.FindProperty("scrollingMode");

			alignment = serializedObject.FindProperty("alignment");
			alignmentGuide = serializedObject.FindProperty("alignmentGuide");
			forceScrolling = serializedObject.FindProperty("forceScrolling");
			updateTransforms = serializedObject.FindProperty("updateTransforms");
			alwaysUpdate = serializedObject.FindProperty("alwaysUpdate");

			navigationScrollingTime = serializedObject.FindProperty("navigationScrollingTime");
			navigationScrollingCurve = serializedObject.FindProperty("navigationScrollingCurve");
			initOnStart = serializedObject.FindProperty("initOnStart");
			autoAddElementComponentToSelectablesOnInit = serializedObject.FindProperty("autoAddElementComponentToSelectablesOnInit");

			selectionChangedTrigger = serializedObject.FindProperty("selectionChangedTrigger");
			selectedBool = serializedObject.FindProperty("selectedBool");
			selectionInt = serializedObject.FindProperty("selectionInt");
			selectionSubmittedTrigger = serializedObject.FindProperty("selectionSubmittedTrigger");
			selectionCancelledTrigger = serializedObject.FindProperty("selectionCancelledTrigger");

			silentOnInitialSelection = serializedObject.FindProperty("silentOnInitialSelection");
			onSelectionChanged = serializedObject.FindProperty("onSelectionChanged");
			onListSelected = serializedObject.FindProperty("onListSelected");
			onListDeselected = serializedObject.FindProperty("onListDeselected");
			onSubmit = serializedObject.FindProperty("onSubmit");
			onCancel = serializedObject.FindProperty("onCancel");
			onListSelectedAsBoolean = serializedObject.FindProperty("onListSelectedAsBoolean");
			onListDeselectedAsBoolean = serializedObject.FindProperty("onListDeselectedAsBoolean");

			var path = AssetDatabase.GetAssetPath( MonoScript.FromScriptableObject( this ) );
			headerTexture = AssetDatabase.LoadAssetAtPath<Texture>( System.IO.Path.GetDirectoryName( path ) + "/Images/SSVHeader.png" );
		}

		bool _displayInfo;
		string _info = "Selection ScrollView Component\n" +
			"Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.\n" +
			"Twitter : @UnityCoach\n" +
			"More information on http://unitycoach.ca";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_displayInfo = EditorGUILayout.Foldout (_displayInfo, "Information");
			if (_displayInfo)
				EditorGUILayout.HelpBox (_info, MessageType.None);

			GUILayout.Space( 5.0f );

			var headerRect = GUILayoutUtility.GetRect( 0.0f, 5.0f );
			headerRect.width = headerTexture.width * 0.5f;
			headerRect.height = headerTexture.height * 0.5f;
			GUILayout.Space( headerRect.height );
			GUI.DrawTexture( headerRect, headerTexture );

			EditorGUILayout.PropertyField(scrollingMode);

			EditorGUILayout.PropertyField(alignment);

			if (alignment.enumValueIndex != 0)
			{
				EditorGUILayout.PropertyField(alignmentGuide);
				EditorGUILayout.PropertyField(forceScrolling);
				EditorGUILayout.PropertyField(updateTransforms);
				EditorGUILayout.PropertyField(alwaysUpdate);
			}

			EditorGUILayout.PropertyField(navigationScrollingTime);
			EditorGUILayout.PropertyField(navigationScrollingCurve);

			if (((SelectionScrollView)(serializedObject.targetObject)).Content.GetComponent<HorizontalOrVerticalLayoutGroup>() == null)
				EditorGUILayout.HelpBox ("No Horizontal Or Vertical Layout Group found on Content", MessageType.Info);

			EditorGUILayout.PropertyField(initOnStart);
			EditorGUILayout.PropertyField(autoAddElementComponentToSelectablesOnInit);

			if (((SelectionScrollView)(serializedObject.targetObject)).Content.GetComponentInChildren<SelectionScrollViewElement>() == null)
				EditorGUILayout.HelpBox ("No Selection ScrollView Element found in Content", MessageType.Info);
			
			if (((SelectionScrollView)(serializedObject.targetObject)).Content.GetComponentInChildren<Selectable>() == null)
			{
				EditorGUILayout.HelpBox ("No Selectable found in Content", MessageType.Info);
			}
			else if (GUILayout.Button("Add Selection ScrollView Element to Selectable Children", EditorStyles.miniButton))
			{
				((SelectionScrollView)this.target).AddSelectionScrollViewElementToSelectableChildren ();
			}
				
			EditorGUILayout.PropertyField(selectionChangedTrigger);
			EditorGUILayout.PropertyField(selectedBool);
			EditorGUILayout.PropertyField(selectionInt);
			EditorGUILayout.PropertyField(selectionSubmittedTrigger);
			EditorGUILayout.PropertyField(selectionCancelledTrigger);

			if (this.targets.Length == 1)
			{
				if (GUILayout.Button("Generate Animator Controller", EditorStyles.miniButton))
				{
					serializedObject.ApplyModifiedProperties();

					// parameters
					string selectionChangedTrigger = ((SelectionScrollView)this.target).selectionChangedTrigger;
					string selectedBool = ((SelectionScrollView)this.target).selectedBool;
					string selectionInt = ((SelectionScrollView)this.target).selectionInt;
					string selectionSubmittedTrigger = ((SelectionScrollView)this.target).selectionSubmittedTrigger;
					string selectionCancelledTrigger = ((SelectionScrollView)this.target).selectionCancelledTrigger;

					RuntimeAnimatorController controller = ScrollViewAnimatorController.GenerateAnimatorController (selectionChangedTrigger, selectedBool, selectionInt, selectionSubmittedTrigger, selectionCancelledTrigger);;

					if (controller)
					{
						Animator _animator = ((SelectionScrollView)this.target).GetComponent<Animator>();

						int undoGroup = Undo.GetCurrentGroup();
						Undo.SetCurrentGroupName ("Set Animator Controller");

						if (!_animator)
							_animator = Undo.AddComponent<Animator>(((SelectionScrollView)this.target).gameObject);

						Undo.RecordObject(_animator, "Set Animator Controller");
						_animator.runtimeAnimatorController = controller;
						Undo.CollapseUndoOperations (undoGroup);
					}
				}
			}
			else
			{
				EditorGUILayout.HelpBox ("Animator Controller Generation not supported on multiple objects.", MessageType.Info);
			}

			EditorGUILayout.PropertyField(silentOnInitialSelection);
			if (onSelectionChanged != null)
			{
				EditorGUILayout.PropertyField(onSelectionChanged);
				EditorGUILayout.PropertyField(onListSelected);
				EditorGUILayout.PropertyField(onListDeselected);
				EditorGUILayout.PropertyField(onSubmit);
				EditorGUILayout.PropertyField(onCancel);
			}
			if (onListSelectedAsBoolean != null)
			{
				EditorGUILayout.PropertyField(onListSelectedAsBoolean);
				EditorGUILayout.PropertyField(onListDeselectedAsBoolean);
			}

			serializedObject.ApplyModifiedProperties();
		}

		#region Static Methods
		[MenuItem ("CONTEXT/ScrollRect/Add Selection ScrollView")]
		static void AddSSV (MenuCommand command)
		{
			ScrollRect obj = (ScrollRect)command.context;
			Undo.SetCurrentGroupName ("Add Selection ScrollView");
			Undo.AddComponent<SelectionScrollView>(obj.gameObject);
		}
		#endregion
	}
}