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

      var first = (position.width - 20f) * 0.7f;
      var second = (position.width - 20f) * 0.3f;
      var third = 20f;

      var sceneProperty = property.FindPropertyRelative("SceneName");
      var propertyPosition = position;
      propertyPosition.width = first;
      EditorGUI.PropertyField(propertyPosition, sceneProperty);
      position.xMin += first;
      
      var lastIndex = Array.IndexOf(_scenes, sceneProperty.stringValue);
      var popupPosition = position;
      popupPosition.width = second;
      var index = EditorGUI.Popup(popupPosition, lastIndex, _scenes);
      if (index != lastIndex)
      {
        sceneProperty.stringValue = _scenes[index];
        sceneProperty.serializedObject.ApplyModifiedProperties();
      }

      position.xMin += second + 2;
      var buttonPosition = position;
      buttonPosition.width = third - 2;
      if (GUI.Button(buttonPosition, "R", EditorStyles.miniButton))
      {
        _scenes = null;
      }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return 16f;
    }
  }
}