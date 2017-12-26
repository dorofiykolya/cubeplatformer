using System.Collections.Generic;
using Game.Components;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Utils.Editor;

namespace Game.Editor
{
  //[CustomPropertyDrawer(typeof(GameLevelData))]
  public class GameLevelPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var nameProperty = property.FindPropertyRelative("Name");
      EditorGUILayout.BeginVertical();
      EditorUtils.BeginVerticalHeader(nameProperty != null ? nameProperty.stringValue : "null");
      if (nameProperty != null)
      {
        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Description"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("EnvironmentPrefab"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("LevelPrefab"));

        var sceneProperty = property.FindPropertyRelative("Scene");
        var scenes = new List<string>();
        scenes.Add("[NULL]");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
          var scene = SceneManager.GetSceneByBuildIndex(i);
          scenes.Add(scene.name);
        }

        var index = EditorGUILayout.Popup("Scene", scenes.IndexOf(sceneProperty.stringValue), scenes.ToArray());
        sceneProperty.stringValue = index < scenes.Count && index >= 0 ? scenes[index] : "";
        sceneProperty.serializedObject.ApplyModifiedProperties();
      }
      EditorUtils.EndVerticalHeader();
    }

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //{
    //  return 0f;
    //}
  }
}