//
//  SelectableCustomEditor.cs
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
	[CustomEditor(typeof(SelectableEvents))]
	[CanEditMultipleObjects]
	public class SelectableEventsEditor : UnityEditor.Editor
	{
		protected void OnEnable()
		{
		}

		bool _displayInfo;
		string _info = "SelectableEvents Component\n" +
			"Copyright (c) 2017 Frederic Moreau, Jikkou Publishing Inc.\n" +
			"Twitter : @UnityCoach\n" +
			"More information on http://unitycoach.ca";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_displayInfo = EditorGUILayout.Foldout (_displayInfo, "Information");
			if (_displayInfo)
				EditorGUILayout.HelpBox (_info, MessageType.None);

			base.DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}

		#region Static Methods
		[MenuItem ("CONTEXT/Selectable/Add Unity Events")]
		static void ConvertSelectable (MenuCommand command)
		{
			Selectable selectable = (Selectable)command.context;

			int undoGroup = UnityEditor.Undo.GetCurrentGroup();
			Undo.SetCurrentGroupName ("Add Unity Events to Selectable");

			selectable.gameObject.AddComponent<SelectableEvents>();

			Undo.CollapseUndoOperations (undoGroup);
		}

		[MenuItem ("CONTEXT/Selectable/Add Unity Events", true)]
		static bool ConvertSelectable_Validate (MenuCommand command)
		{
			return (((Selectable)command.context) != null);
		}
		#endregion
	}
}