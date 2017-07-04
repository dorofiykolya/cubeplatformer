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

      DrawSelectCubePlatforms();
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
      var cell = list[0];
      if (list.Length == 1)
      {
        EditorGUILayout.LabelField("Position:", cell.Position.ToString());
        cell.CellType = (CellType)EditorGUILayout.EnumPopup("Type:", cell.CellType);
        EditorGUILayout.LabelField("Name:", cell.CellInfo.Name ?? "");
        EditorGUILayout.LabelField("Prefab:", cell.CellInfo.Prefab ? cell.CellInfo.Prefab.ToString() : "null");
      }
      EditorGUILayout.BeginHorizontal(EditorUtils.Styles.ProgressBarBack);
      if (GUILayout.Button("SelectLevel", EditorUtils.Styles.minibuttonleft))
      {
        Selection.activeGameObject = cell.Level.gameObject;
      }
      if (GUILayout.Button("LevelEditor", EditorUtils.Styles.minibuttonright))
      {
        LevelEditorWindow.Open(cell.Level);
      }
      EditorGUILayout.EndHorizontal();


      EditorUtils.PushColor();
      var editorPrefsKey = GetType().FullName + ".Preset";
      if (_preset == null) GUI.color = Color.red;
      if (_preset == null)
      {
        var path = EditorPrefs.GetString(editorPrefsKey);
        if (path != null)
        {
          _preset = AssetDatabase.LoadAssetAtPath<CellPreset>(path);
          if (_preset == null)
          {
            EditorPrefs.DeleteKey(editorPrefsKey);
          }
        }
      }
      var preset = EditorGUILayout.ObjectField("Preset:", _preset, typeof(CellPreset), false) as CellPreset;
      if (preset != _preset)
      {
        _preset = preset;

        EditorPrefs.SetString(editorPrefsKey, AssetDatabase.GetAssetPath(_preset));
      }
      EditorUtils.PopColor();
      if (_preset != null)
      {
        EditorUtils.Header("Type: " + ((list.Select(s => s.CellType).Distinct().Count() == 1) ? list[0].CellType.ToString() : " --"));

        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        var index = 0;
        _findText = EditorGUILayout.TextField(_findText, (GUIStyle)EditorUtils.Styles.SearchTextField);
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        var presets =
          _preset.Cells.Where(
          (p) => string.IsNullOrEmpty(_findText) || (p.Name != null && p.Name.ToLowerInvariant().Contains(_findText.ToLowerInvariant())) || p.Type.ToString().ToLowerInvariant().Contains(_findText.ToLowerInvariant()));
        if (presets.Any())
        {
          foreach (var presetCell in presets)
          {
            EditorUtils.PushColor();
            if (list.All(s => s.CellInfo == presetCell && s.CellInfo.Type == s.CellType))
            {
              GUI.color = Color.green;
            }
            var style = ((GUIStyle)EditorUtils.Styles.minibutton);
            style.alignment = TextAnchor.MiddleLeft;
            var pressetName = (presetCell.Name ?? "").PadRight(12);
            var pressetType = presetCell.Type.ToString().PadRight(12);
            var pressetPrefab = presetCell.Prefab;
            if (GUILayout.Button(string.Format("{0} {1} \t {2} \t {3}", index, pressetName, pressetType, pressetPrefab), style))
            {
              foreach (var component in list)
              {
                component.SetContent(presetCell);
              }
            }
            EditorUtils.PopColor();
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
    }
  }
}