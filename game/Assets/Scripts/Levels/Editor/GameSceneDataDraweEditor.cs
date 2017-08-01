using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Game.Editor
{
  [CustomPropertyDrawer(typeof(GameSceneData))]
  public class GameSceneDataDraweEditor : PropertyDrawer
  {
    private static string[] _scenes;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (_scenes == null)
      {
        _scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        for (int i = 0; i < _scenes.Length; i++)
        {
          _scenes[i] = Path.GetFileNameWithoutExtension(_scenes[i]);
        }
      }
      GUILayout.Space(-16f);
      EditorGUILayout.BeginHorizontal();
      var sceneProperty = property.FindPropertyRelative("Name");
      EditorGUILayout.PropertyField(sceneProperty);
      var lastIndex = Array.IndexOf(_scenes, sceneProperty.stringValue);
      var index = EditorGUILayout.Popup(lastIndex, _scenes, GUILayout.MaxWidth(150));
      if (index != lastIndex)
      {
        sceneProperty.stringValue = _scenes[index];
        sceneProperty.serializedObject.ApplyModifiedProperties();
      }
      if (GUILayout.Button("R", EditorStyles.miniButton, GUILayout.Width(18f)))
      {
        _scenes = null;
      }
      EditorGUILayout.EndHorizontal();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return 0;
    }
  }
}