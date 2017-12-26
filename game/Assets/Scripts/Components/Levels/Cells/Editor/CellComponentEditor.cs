using System;
using System.Linq;
using Game.Components;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using Utils.Editor;

namespace Game.Editor
{
  [CustomEditor(typeof(CellComponent)), CanEditMultipleObjects]
  public class CellComponentEditor : Editor<CellComponent>
  {
    private static CellPreset _preset;
    private static bool _inherited;
    private static bool _autoDirection = true;

    private string _findText;

    public override void OnInspectorGUI()
    {
      _inherited = EditorUtils.FoldoutHeader("inherited", _inherited);
      if (_inherited)
      {
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        base.OnInspectorGUI();
        EditorGUILayout.EndVertical();
      }

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
        var lastType = cell.CellType;
        EditorGUILayout.LabelField("Position:", cell.Position.ToString());
        cell.CellType = (CellType)EditorGUILayout.EnumPopup("Type:", cell.CellType);
        EditorGUILayout.LabelField("Name:", cell.CellInfo.Name ?? "");
        EditorGUILayout.LabelField("Prefab:", cell.CellInfo.Prefab ? cell.CellInfo.Prefab.ToString() : "null");

        if (lastType == CellType.Guard && cell.CellType != CellType.Guard)
        {
          var guard = cell.GetComponent<CellGuardComponent>();
          if (guard != null) GameObject.DestroyImmediate(guard);
        }
        else if (lastType != CellType.Guard && cell.CellType == CellType.Guard)
        {
          cell.gameObject.AddComponent<CellGuardComponent>();
        }
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
      GUILayout.Space(10f);
      if (GUILayout.Button("UpdateContent", EditorUtils.Styles.minibutton))
      {
        foreach (var cellComponent in list)
        {
          cellComponent.UpdateContent();
        }
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
        EditorGUILayout.BeginHorizontal(EditorUtils.Styles.ProgressBarBack);
        if (GUILayout.Button("UpdateContentFromPreset", EditorUtils.Styles.minibutton))
        {
          foreach (var component in list)
          {
            if (_preset.Cells.Any(c => c.Name == component.CellInfo.Name && c.Type == component.CellInfo.Type))
            {
              component.SetContent(_preset.Cells.First(c => c.Name == component.CellInfo.Name && c.Type == component.CellInfo.Type));
            }
            else if (_preset.Cells.Any(c => c.Type == component.CellInfo.Type))
            {
              component.SetContent(_preset.Cells.First(c => c.Type == component.CellInfo.Type));
            }
          }
        }
        EditorGUILayout.EndHorizontal();

        EditorUtils.Header("Type: " + ((list.Select(s => s.CellType).Distinct().Count() == 1) ? list[0].CellType.ToString() : " --"));

        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        _autoDirection = EditorGUILayout.Toggle("Auto direction", _autoDirection);
        var index = 0;
        _findText = EditorGUILayout.TextField(_findText, (GUIStyle)EditorUtils.Styles.SearchTextField);
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        var presets =
          _preset.Cells.Where(
          (p) => string.IsNullOrEmpty(_findText) || (p.Name != null && p.Name.ToLowerInvariant().Contains(_findText.ToLowerInvariant())) || p.Type.ToString().ToLowerInvariant().Contains(_findText.ToLowerInvariant()));
        if (presets.Any())
        {
          var maxName = 0;
          foreach (var info in presets)
          {
            var str = string.Format("{0} {1}", index++, info.Name ?? "");
            if (str.Length > maxName) maxName = str.Length;
          }
          
          index = 0;

          foreach (var presetCell in presets)
          {
            EditorUtils.PushColor();
            if (list.All(s => s.CellInfo == presetCell && s.CellInfo.Type == s.CellType))
            {
              GUI.color = Color.green;
            }
            var style = ((GUIStyle)EditorUtils.Styles.minibutton);
            style.alignment = TextAnchor.MiddleLeft;
            var pressetName = ((presetCell.Name ?? "") + "   ").PadRight(maxName, '_');
            var pressetType = presetCell.Type.ToString();
            var pressetPrefab = presetCell.Prefab;
            var pressetDirection = presetCell.Direction;
            var finalPresetName = string.Format("{0} {1}", index, pressetName);
            //if (finalPresetName.Length < maxName) finalPresetName += string.Join("-", new string[maxName - finalPresetName.Length]);
            if (GUILayout.Button(string.Format("{0} \t {1} \t {2} \t {3}", finalPresetName, pressetType, pressetDirection, pressetPrefab), style))
            {
              if (list.Length == 1 || !_autoDirection || presetCell.Direction != CellDirection.None)
              {
                foreach (var component in list)
                {
                  component.SetContent(presetCell);
                }
              }
              else
              {
                foreach (var component in list)
                {
                  component.SetContent(presetCell);
                }

                foreach (var component in list)
                {
                  var level = component.Level;
                  if (IsAutoDirection(component, CellDirection.Right))
                  {
                    component.SetContent(_preset.GetByType(presetCell.Type, CellDirection.Right));
                  }
                  else if (IsAutoDirection(component, CellDirection.Left))
                  {
                    component.SetContent(_preset.GetByType(presetCell.Type, CellDirection.Left));
                  }
                  else if (IsAutoDirection(component, CellDirection.Bottom))
                  {
                    component.SetContent(_preset.GetByType(presetCell.Type, CellDirection.Bottom));
                  }
                  else if (IsAutoDirection(component, CellDirection.Top))
                  {
                    component.SetContent(_preset.GetByType(presetCell.Type, CellDirection.Top));
                  }
                }
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

    private static bool IsAutoDirection(CellComponent component, CellDirection direction)
    {
      var level = component.Level;
      var next = level.GetCellType(component.Position.GetPosition(direction));
      var prev = level.GetCellType(component.Position.GetPosition(direction.Invert()));
      return next != component.CellType && prev == component.CellType;
    }
  }
}