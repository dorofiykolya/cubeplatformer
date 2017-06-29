using UnityEngine;
using UnityEditor;
using Utils.Editor;

namespace Game.Editor
{
  [CustomPropertyDrawer(typeof(Position))]
  public class PositionPropertyDrawerEditor : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var x = property.FindPropertyRelative("X");
      var y = property.FindPropertyRelative("Y");
      var z = property.FindPropertyRelative("Z");
      EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
      EditorGUILayout.BeginHorizontal();
      GUILayout.Label(property.displayName);
      GUILayout.Label("X", GUILayout.Width(12f));
      x.intValue = EditorGUILayout.IntField(x.intValue);
      GUILayout.Label("Y", GUILayout.Width(12f));
      y.intValue = EditorGUILayout.IntField(y.intValue);
      GUILayout.Label("Z", GUILayout.Width(12f));
      z.intValue = EditorGUILayout.IntField(z.intValue);
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.EndVertical();
      property.serializedObject.ApplyModifiedProperties();
    }
  }
}