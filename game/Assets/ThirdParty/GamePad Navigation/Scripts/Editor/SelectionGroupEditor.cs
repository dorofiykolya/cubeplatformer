//
//  SelectionGroupEditor.cs
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
using UnityEditor.UI;
using UnityCoach.GamePadNavigation;

namespace UnityCoach.GamePadNavigation.Editor
{
	[CustomEditor (typeof (SelectionGroup))]
	[CanEditMultipleObjects]
	public class SelectionGroupEditor : UnityEditor.Editor
	{
		#region Selector
		SerializedProperty content;
		SerializedProperty _defaultSelection;
		SerializedProperty initOnStart;
		#endregion

		#region Navigation
		SerializedProperty enterOnSelect;
		SerializedProperty autoExitNavigationMode;
		SerializedProperty exitLeft;
		SerializedProperty exitRight;
		SerializedProperty exitUp;
		SerializedProperty exitDown;
		SerializedProperty followNavigationOnExit;
		SerializedProperty exitNavigationTarget;
		SerializedProperty selectionSaveMode;
		#endregion

		#region Unity Events
		SerializedProperty silentOnInit;
		SerializedProperty onSelectionChanged;
		SerializedProperty onEnter;
		SerializedProperty onExit;
		SerializedProperty onSubmit;
		#endregion

		Texture headerTexture;

		protected void OnEnable()
		{
			content = serializedObject.FindProperty("content");
			_defaultSelection = serializedObject.FindProperty("_defaultSelection");
			initOnStart = serializedObject.FindProperty("initOnStart");

			enterOnSelect = serializedObject.FindProperty("enterOnSelect");
			autoExitNavigationMode = serializedObject.FindProperty("autoExitNavigationMode");
			exitLeft = serializedObject.FindProperty("exitLeft");
			exitRight = serializedObject.FindProperty("exitRight");
			exitUp = serializedObject.FindProperty("exitUp");
			exitDown = serializedObject.FindProperty("exitDown");
			followNavigationOnExit = serializedObject.FindProperty("followNavigationOnExit");
			exitNavigationTarget = serializedObject.FindProperty("exitNavigationTarget");
			selectionSaveMode = serializedObject.FindProperty("selectionSaveMode");

			silentOnInit = serializedObject.FindProperty("silentOnInit");
			onSelectionChanged = serializedObject.FindProperty("onSelectionChanged");
			onEnter = serializedObject.FindProperty("onEnter");
			onExit = serializedObject.FindProperty("onExit");
			onSubmit = serializedObject.FindProperty("onSubmit");

			var path = AssetDatabase.GetAssetPath( MonoScript.FromScriptableObject( this ) );
			headerTexture = AssetDatabase.LoadAssetAtPath<Texture>( System.IO.Path.GetDirectoryName( path ) + "/Images/SGHeader.png" );
		}

		bool _displayInfo;
		string _info = "Selection Group Component\n" +
			"Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.\n" +
			"Twitter : @UnityCoach\n" +
			"More information on http://unitycoach.ca";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_displayInfo = EditorGUILayout.Foldout (_displayInfo, "Information");
			if (_displayInfo)
				EditorGUILayout.HelpBox (_info, MessageType.None);

//			DrawDefaultInspector ();

			GUILayout.Space( 5.0f );

			var headerRect = GUILayoutUtility.GetRect( 0.0f, 5.0f );
			headerRect.width = headerTexture.width * 0.5f;
			headerRect.height = headerTexture.height * 0.5f;
			GUILayout.Space( headerRect.height );
			GUI.DrawTexture( headerRect, headerTexture );

			EditorGUILayout.PropertyField(content);
			EditorGUILayout.PropertyField(_defaultSelection);
			EditorGUILayout.PropertyField(initOnStart);

			EditorGUILayout.PropertyField(enterOnSelect);
			EditorGUILayout.PropertyField(autoExitNavigationMode);

			if (((SelectionGroup)(serializedObject.targetObject)).autoExitNavigationMode == Navigation.Mode.Explicit)
			{
				EditorGUILayout.PropertyField(exitLeft);
				EditorGUILayout.PropertyField(exitRight);
				EditorGUILayout.PropertyField(exitUp);
				EditorGUILayout.PropertyField(exitDown);
			}

			EditorGUILayout.PropertyField(followNavigationOnExit);
			EditorGUILayout.PropertyField(exitNavigationTarget);
			EditorGUILayout.PropertyField(selectionSaveMode);

			EditorGUILayout.PropertyField(silentOnInit);
			EditorGUILayout.PropertyField(onSelectionChanged);
			EditorGUILayout.PropertyField(onEnter);
			EditorGUILayout.PropertyField(onExit);
			EditorGUILayout.PropertyField(onSubmit);

			serializedObject.ApplyModifiedProperties();
		}
	}
}