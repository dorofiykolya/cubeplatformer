using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using Utils.Editor;

namespace Game.Views.Editor
{
  [CustomEditor(typeof(CellComponent)), CanEditMultipleObjects]
  public class CellComponentEditor : Editor<CellComponent>
  {
    private static CellPreset _preset;
    private static readonly AnimBool _inherited = new AnimBool(false);

    private string _findText;

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

      EditorUtils.PushColor();
      if (_preset == null) GUI.color = Color.red;
      _preset = EditorGUILayout.ObjectField("Preset:", _preset, typeof(CellPreset), false) as CellPreset;
      EditorUtils.PopColor();

      if (_preset != null)
      {
        DrawSelectCubePlatforms();
      }
    }

    private void DrawSelectCubePlatforms()
    {
      if (targets != null && targets.Length > 1)
      {
        DrawSelectCubePlatformMulti(Targets);
      }
      else
      {
        DrawSelectCubePlatformMulti(new CellComponent[] { Target });
      }
    }

    private void DrawSelectCubePlatformMulti(CellComponent[] list)
    {
      if (list == null || list.Length == 0) return;
      if (list.Length == 1)
      {
        var cell = list[0];
        EditorUtils.BeginHorizontal(true);
        if (GUILayout.Button("SelectLevel", EditorUtils.Styles.minibuttonleft))
        {
          Selection.activeGameObject = cell.Level.gameObject;
        }
        if (GUILayout.Button("LevelEditor", EditorUtils.Styles.minibuttonright))
        {
          LevelEditorWindow.Open(cell.Level);
        }
        EditorUtils.EndHorizontal();
        EditorGUILayout.LabelField("Position:", cell.Position.ToString());
      }
      EditorUtils.Header("Type");
      EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
      var index = 0;
      _findText = EditorGUILayout.TextField(_findText, (GUIStyle)EditorUtils.Styles.SearchTextField);
      EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
      var presets =
        _preset.Cells.Where(
          (p) => string.IsNullOrEmpty(_findText) || p.Name.ToLowerInvariant().Contains(_findText.ToLowerInvariant()) || p.Type.ToString().ToLowerInvariant().Contains(_findText.ToLowerInvariant()));
      if (presets.Any())
      {
        foreach (var presetCell in presets)
        {
          var style = ((GUIStyle)EditorUtils.Styles.minibutton);
          style.alignment = TextAnchor.MiddleLeft;
          if (GUILayout.Button(string.Format("{0} {1} \t {2} {3}", index, presetCell.Name, presetCell.Type, presetCell.Prefab), style))
          {
            foreach (var component in list)
            {
              component.SetContent(presetCell);
            }
          }
          index++;
        }
      }
      else
      {
        GUILayout.Label("Preset is empty");
      }
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndVertical();
    }

    private void DrawSelectCubePlatform(CellComponent current, CellComponent runner)
    {
      if (target == null) return;

      var cubePlatform = current;
      var newList = _preset.Cells;
      var nameList = new string[newList.Length];
      var empty = _preset.Cells.FirstOrDefault(s => s.Type == CellType.Empty);
      var index = 0;
      foreach (var platformInfo in newList)
      {
        nameList[index] = string.Format("{0} {1} \t {2} {3}", index, platformInfo.Name, platformInfo.Type, platformInfo.Prefab);
        index++;
      }
      EditorUtils.BeginHorizontal(true);
      if (GUILayout.Button("SelectLevel", EditorUtils.Styles.minibuttonleft))
      {
        Selection.activeGameObject = cubePlatform.Level.gameObject;
      }
      if (GUILayout.Button("LevelEditor", EditorUtils.Styles.minibuttonright))
      {
        LevelEditorWindow.Open(cubePlatform.Level);
      }
      EditorUtils.EndHorizontal();
      EditorGUILayout.LabelField("Position:", cubePlatform.Position.ToString());

    }
  }
}