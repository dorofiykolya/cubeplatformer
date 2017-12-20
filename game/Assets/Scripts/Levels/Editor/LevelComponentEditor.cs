using System.Collections.Generic;
using System.IO;
using Game.Logics.Classic;
using Game.Views.Components;
using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace Game.Views.Editor
{
  [CustomEditor(typeof(LevelComponent))]
  public class LevelComponentEditor : Editor<LevelComponent>
  {
    private static bool _inherited;

    public override void OnInspectorGUI()
    {
      _inherited = EditorUtils.FoldoutHeader("inherited", _inherited);
      if (_inherited)
      {
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        base.OnInspectorGUI();
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.LabelField("Size:", Target.Size.ToString());


      EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
      EditorGUILayout.BeginHorizontal(EditorUtils.Styles.ProgressBarBack);
      if (GUILayout.Button("UpdateConverter", EditorUtils.Styles.minibuttonleft))
      {
        Target.UpdateConverter();
      }
      bool isPrefabInstance = PrefabUtility.GetPrefabParent(Target.gameObject) != null && PrefabUtility.GetPrefabObject(Target.gameObject.transform) != null;
      bool isPrefabOriginal = PrefabUtility.GetPrefabParent(Target.gameObject) == null && PrefabUtility.GetPrefabObject(Target.gameObject.transform) != null;
      bool isDisconnectedPrefabInstance = PrefabUtility.GetPrefabParent(Target.gameObject) != null && PrefabUtility.GetPrefabObject(Target.gameObject.transform) == null;

      if (isPrefabInstance)
      {
        if (GUILayout.Button("Disconnect Prefab", EditorUtils.Styles.minibuttonright))
        {
          PrefabUtility.DisconnectPrefabInstance(Target.gameObject);
        }
      }
      else if (isDisconnectedPrefabInstance)
      {
        if (GUILayout.Button("Connect to Prefab", EditorUtils.Styles.minibuttonright))
        {
          PrefabUtility.ConnectGameObjectToPrefab(Target.gameObject, (GameObject)PrefabUtility.GetPrefabParent(Target.gameObject));
        }
      }
      else if (isPrefabOriginal)
      {
        if (GUILayout.Button("Edit In Scene", EditorUtils.Styles.minibuttonright))
        {
          Selection.activeGameObject = (GameObject)PrefabUtility.InstantiatePrefab(Target.gameObject);
        }
      }

      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal(EditorUtils.Styles.ProgressBarBack);
      if (GUILayout.Button("UpdateCellsContent", EditorUtils.Styles.minibutton))
      {
        foreach (var cell in Target.GetComponentsInChildren<CellComponent>(true))
        {
          if (cell.CellInfo.Prefab != null)
          {
            cell.UpdateContent();
          }
        }
      }
      if (GUILayout.Button("FixDisconnectedCellsContent", EditorUtils.Styles.minibutton))
      {
        foreach (var cell in Target.GetComponentsInChildren<CellComponent>(true))
        {
          var listToRemove = new List<GameObject>();
          foreach (Transform child in cell.transform)
          {
            var childContent = child.GetComponent<CellContentComponent>();
            if (!(childContent != null && cell.Content == childContent))
            {
              listToRemove.Add(child.gameObject);
            }
          }
          foreach (var gameObject in listToRemove)
          {
            DestroyImmediate(gameObject);
          }
        }
      }
      EditorGUILayout.EndHorizontal();
      EditorUtils.BeginVerticalHeader("Serialize");
      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("ToClassic", EditorUtils.Styles.minibutton))
      {
        var serialized = ClassicLogicLevelConverter.Convert(Target);
        var path = EditorUtility.SaveFilePanelInProject("Save serialized file", "classic_level", "txt", "Please enter a file name to save the level to");
        if (path.Length != 0)
        {
          File.WriteAllText(path, serialized);
          AssetDatabase.Refresh();
        }
      }
      GUI.tooltip = "not implemented";
      if (GUILayout.Button("ToCSV", EditorUtils.Styles.minibutton))
      {

      }
      GUI.tooltip = string.Empty;
      EditorGUILayout.EndHorizontal();
      EditorUtils.EndVerticalHeader();
      EditorGUILayout.EndVertical();

      if (GUILayout.Button("LevelEditor"))
      {
        LevelEditorWindow.Open(Target);
      }
    }
  }
}