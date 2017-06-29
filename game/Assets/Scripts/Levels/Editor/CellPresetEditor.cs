using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace Game.Editor
{
  [CustomEditor(typeof(CellPreset))]
  public class CellPresetEditor : Editor<CellPreset>
  {
    private static bool _foldout;

    public override void OnInspectorGUI()
    {
      Target.Name = EditorGUILayout.TextField("Name:", string.IsNullOrEmpty(Target.Name) ? Target.name : Target.Name);
      _foldout = BeginFade(_foldout, "Preset: " + Target.Name);
      DrawContent(Target);
      EndFade();
    }

    private static void DrawContent(CellPreset Target)
    {
      if (Target.Cells == null || Target.Cells.Length == 0)
      {
        Target.Cells = new[] { new CellInfo() };
      }
      var selected = true;
      var index = 0;
      var stop = false;
      foreach (var platform in Target.Cells)
      {
        var np = platform;
        GUILayout.BeginVertical(selected ? EditorUtils.Styles.ProgressBarBack : EditorUtils.Styles.ProgressBarText);
        GUILayout.BeginHorizontal();
        np.Name = EditorGUILayout.TextField(platform.Name);
        np.Type = (CellType)EditorGUILayout.EnumPopup(platform.Type);
        if (string.IsNullOrEmpty(np.Name) && platform.Prefab != null)
        {
          Target.Cells[index].Name = platform.Prefab.name;
        }
        if (GUILayout.Button("", EditorUtils.Styles.OL_Minus, GUILayout.Width(15f)))
        {
          var list = Target.Cells.ToList();
          list.RemoveAt(index);
          Target.Cells = list.ToArray();
          stop = true;
        }
        GUILayout.EndHorizontal();
        np.Prefab = (GameObject)EditorGUILayout.ObjectField(np.Prefab, typeof(GameObject), false);

        if (!stop && GUI.changed)
        {
          Target.Cells[index] = np;
          stop = true;
        }

        GUILayout.EndVertical();
        selected = !selected;
        index++;
        if (stop) break;
      }

      if (GUILayout.Button("", EditorUtils.Styles.OL_Plus))
      {
        Array.Resize(ref Target.Cells, Target.Cells.Length + 1);
      }
    }

    [CustomPropertyDrawer(typeof(CellPreset), true)]
    public class CellPresetPropertyDrawer : PropertyDrawer
    {
      private static bool _foldout;

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        EditorUtils.Header("Preset", -5f);
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        EditorGUILayout.ObjectField(property, new GUIContent("Preset:"));
        if (!(property == null || property.objectReferenceValue == null))
        {
          var target = (CellPreset)property.objectReferenceValue;
          _foldout = EditorUtils.FoldoutHeader("Preset: " + target.Name, _foldout);
          if (_foldout)
          {
            DrawContent((CellPreset)property.objectReferenceValue);
          }
        }
        EditorGUILayout.EndVertical();
      }
    }
  }
}