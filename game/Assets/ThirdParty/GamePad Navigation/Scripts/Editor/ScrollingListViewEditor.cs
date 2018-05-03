//
//  ScrollingListViewEditor.cs
//
//  Author:
//       Frederic Moreau <info@unitycoach.ca>
//
//  Copyright (c) 2018 Frederic Moreau, UnityCoach (Jikkou Publishing Inc.)

//#define JIKKOU_UI_LOOP_CONTENT // ----- WORK IN PROGRESS, UNCOMMENT AT YOUR OWN RISK ----- \\

using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
//using UnityEditor.Animations;
using UnityEditor.UI;
using UnityCoach.GamePadNavigation;

namespace UnityCoach.GamePadNavigation.Editor
{
	[CustomEditor(typeof(ScrollingListView))]
	[CanEditMultipleObjects]
	public class ScrollingListViewEditor : SelectableEditor
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
		SerializedProperty deselectedContentOnExit;
		SerializedProperty useSelectables;
		SerializedProperty defaultSelection;

		#if JIKKOU_UI_LOOP_CONTENT
		SerializedProperty loopContent;
		#endif

		SerializedProperty _scrollThreshold;
		SerializedProperty _dragThreshold;
		#endregion

		#region Animator
		SerializedProperty selectionChangedTrigger;
		SerializedProperty selectedBool;
		SerializedProperty selectionInt;
		SerializedProperty selectionSubmittedTrigger;
		SerializedProperty selectionCancelledTrigger;
		#endregion

		#region Unity Events
		SerializedProperty onSelectionChanged;
		SerializedProperty onListSelected;
		SerializedProperty onListDeselected;
		SerializedProperty onSubmit;
		SerializedProperty onCancel;
		SerializedProperty onListSelectedAsBoolean;
		SerializedProperty onListDeselectedAsBoolean;
		#endregion

		Texture headerTexture;

		protected override void OnEnable()
		{
			base.OnEnable ();

			scrollingMode = serializedObject.FindProperty("scrollingMode");

			alignment = serializedObject.FindProperty("alignment");
			alignmentGuide = serializedObject.FindProperty("alignmentGuide");
			forceScrolling = serializedObject.FindProperty("forceScrolling");
			updateTransforms = serializedObject.FindProperty("updateTransforms");
			alwaysUpdate = serializedObject.FindProperty("alwaysUpdate");

			navigationScrollingTime = serializedObject.FindProperty("navigationScrollingTime");
			navigationScrollingCurve = serializedObject.FindProperty("navigationScrollingCurve");
			initOnStart = serializedObject.FindProperty("initOnStart");
			deselectedContentOnExit = serializedObject.FindProperty("deselectedContentOnExit");
			useSelectables = serializedObject.FindProperty("useSelectables");
			defaultSelection = serializedObject.FindProperty("defaultSelection");

			#if JIKKOU_UI_LOOP_CONTENT
			loopContent = serializedObject.FindProperty("loopContent");
			#endif

			_scrollThreshold = serializedObject.FindProperty("_scrollThreshold");
			_dragThreshold = serializedObject.FindProperty("_dragThreshold");

			selectionChangedTrigger = serializedObject.FindProperty("selectionChangedTrigger");
			selectedBool = serializedObject.FindProperty("selectedBool");
			selectionInt = serializedObject.FindProperty("selectionInt");
			selectionSubmittedTrigger = serializedObject.FindProperty("selectionSubmittedTrigger");
			selectionCancelledTrigger = serializedObject.FindProperty("selectionCancelledTrigger");

			onSelectionChanged = serializedObject.FindProperty("onSelectionChanged");
			onListSelected = serializedObject.FindProperty("onListSelected");
			onListDeselected = serializedObject.FindProperty("onListDeselected");
			onSubmit = serializedObject.FindProperty("onSubmit");
			onCancel = serializedObject.FindProperty("onCancel");
			onListSelectedAsBoolean = serializedObject.FindProperty("onListSelectedAsBoolean");
			onListDeselectedAsBoolean = serializedObject.FindProperty("onListDeselectedAsBoolean");

			var path = AssetDatabase.GetAssetPath( MonoScript.FromScriptableObject( this ) );
			headerTexture = AssetDatabase.LoadAssetAtPath<Texture>( System.IO.Path.GetDirectoryName( path ) + "/Images/SLVHeader.png" );
		}

		bool _displayInfo;
		string _info = "Navigable ScrollView Component\n" +
			"Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.\n" +
			"Twitter : @UnityCoach\n" +
			"More information on http://unitycoach.ca";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_displayInfo = EditorGUILayout.Foldout (_displayInfo, "Information");
			if (_displayInfo)
				EditorGUILayout.HelpBox (_info, MessageType.None);

			base.OnInspectorGUI();

			GUILayout.Space( 5.0f );

			var headerRect = GUILayoutUtility.GetRect( 0.0f, 5.0f );
			headerRect.width = headerTexture.width * 0.5f;
			headerRect.height = headerTexture.height * 0.5f;
			GUILayout.Space( headerRect.height );
			GUI.DrawTexture( headerRect, headerTexture );

			EditorGUILayout.PropertyField (scrollingMode);

			EditorGUILayout.PropertyField(alignment);

			if (alignment.enumValueIndex != 0)
			{
				EditorGUILayout.PropertyField(alignmentGuide);
				EditorGUILayout.PropertyField(forceScrolling);
				EditorGUILayout.PropertyField(updateTransforms);
				EditorGUILayout.PropertyField(alwaysUpdate);
			}

			EditorGUILayout.PropertyField (navigationScrollingTime);
			EditorGUILayout.PropertyField (navigationScrollingCurve);

			if (((ScrollingListView)(serializedObject.targetObject)).Content.GetComponent<HorizontalOrVerticalLayoutGroup>() == null)
				EditorGUILayout.HelpBox ("No Horizontal Or Vertical Layout Group found on Content", MessageType.Info);

			EditorGUILayout.PropertyField (initOnStart);
			EditorGUILayout.PropertyField (deselectedContentOnExit);

			EditorGUILayout.PropertyField (useSelectables);
			EditorGUILayout.PropertyField (defaultSelection);

			#if JIKKOU_UI_LOOP_CONTENT
			EditorGUILayout.PropertyField (loopContent);
			#endif

			EditorGUILayout.PropertyField (_scrollThreshold);
			EditorGUILayout.PropertyField (_dragThreshold);

			bool selectablesFound = ((ScrollingListView)(serializedObject.targetObject)).Content.GetComponentInChildren<Selectable>() != null;

			if (((ScrollingListView)(serializedObject.targetObject)).useSelectables)
			{
				if (!selectablesFound)
					EditorGUILayout.HelpBox ("No Selectable found in Content", MessageType.Info);
			}
			else
			{
				if (((ScrollingListView)(serializedObject.targetObject)).Content.GetComponentInChildren<ScrollingListItemView>() == null)
				{
					EditorGUILayout.HelpBox ("No Scrolling List Item found in Content", MessageType.Info);

					// UNDONE : Selectables AND List Items currently not supported
//					if (GUILayout.Button("Add Scrolling List Item to Selectable Children", EditorStyles.miniButton))
//						((ScrollingListView)this.target).AddNavigationScrollViewElementToSelectableChildren ();
					
					if (selectablesFound)
						if (GUILayout.Button("Convert Selectables to Scrolling List Items", EditorStyles.miniButton))
							((ScrollingListView)this.target).ConvertSelectablesToNavigableScrollViewElements ();
				}
			}

			EditorGUILayout.PropertyField (selectionChangedTrigger);
			EditorGUILayout.PropertyField (selectedBool);
			EditorGUILayout.PropertyField (selectionInt);
			EditorGUILayout.PropertyField (selectionSubmittedTrigger);
			EditorGUILayout.PropertyField (selectionCancelledTrigger);

			if (this.targets.Length == 1)
			{
				if (GUILayout.Button("Generate Animator Controller", EditorStyles.miniButton))
				{
					serializedObject.ApplyModifiedProperties ();

					// parameters
					string selectionChangedTrigger = ((ScrollingListView)this.target).selectionChangedTrigger;
					string selectedBool = ((ScrollingListView)this.target).selectedBool;
					string selectionInt = ((ScrollingListView)this.target).selectionInt;
					string selectionSubmittedTrigger = ((ScrollingListView)this.target).selectionSubmittedTrigger;
					string selectionCancelledTrigger = ((ScrollingListView)this.target).selectionCancelledTrigger;

					RuntimeAnimatorController controller = ScrollViewAnimatorController.GenerateAnimatorController (selectionChangedTrigger, selectedBool, selectionInt, selectionSubmittedTrigger, selectionCancelledTrigger);;

					if (controller)
					{
						Animator _animator = ((ScrollingListView)this.target).GetComponent<Animator>();

						int undoGroup = Undo.GetCurrentGroup();
						Undo.SetCurrentGroupName ("Set Animator Controller");

						if (!_animator)
							_animator = Undo.AddComponent<Animator>(((ScrollingListView)this.target).gameObject);

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
		[MenuItem ("CONTEXT/ScrollRect/Add Scrolling List View")]
		static void AddNSV (MenuCommand command)
		{
			ScrollRect obj = (ScrollRect)command.context;
			Undo.SetCurrentGroupName ("Add Scrolling List View");
			Undo.AddComponent<ScrollingListView>(obj.gameObject);
		}
		#endregion
	}
}