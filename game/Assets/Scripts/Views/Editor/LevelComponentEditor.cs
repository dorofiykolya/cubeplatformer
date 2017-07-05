using Game.Views.Components;
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

      EditorGUILayout.EndVertical();

      if (GUILayout.Button("LevelEditor"))
      {
        LevelEditorWindow.Open(Target);
      }
    }
  }
}