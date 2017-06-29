using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Utils.Editor;

namespace Game.Views.Editor
{
  [CustomEditor(typeof(LevelComponent))]
  public class LevelComponentEditor : Editor<LevelComponent>
  {
    private static readonly AnimBool _inherited = new AnimBool(false);

    private void OnEnable()
    {
      _inherited.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
      _inherited.target = EditorUtils.FoldoutHeader("inherited", _inherited.target);
      if (EditorGUILayout.BeginFadeGroup(_inherited.faded))
      {
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        base.OnInspectorGUI();
        EditorGUILayout.EndVertical();
      }
      EditorGUILayout.EndFadeGroup();

      if (GUILayout.Button("UpdateConverter"))
      {
        Target.UpdateConverter();
      }
    }
  }
}