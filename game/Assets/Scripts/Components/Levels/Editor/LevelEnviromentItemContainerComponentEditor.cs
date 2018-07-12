using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace Game.Components.Levels.Editor
{
  [CustomEditor(typeof(LevelEnviromentItemContainerComponent))]
  public class LevelEnviromentItemContainerComponentEditor : Editor<LevelEnviromentItemContainerComponent>
  {
    private static bool _inherited;

    public override void OnInspectorGUI()
    {
      _inherited = EditorGUILayout.Toggle("Inherited", _inherited);
      if (_inherited)
      {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
      }

      if (Target.Preset == null)
      {
        GUILayout.Label("Error, Preset is Null");
        return;
      }

      EditorUtils.PushColor();
      if (Target.Prefab == null) GUI.color = Color.green;
      if (GUILayout.Button("Empty"))
      {
        Target.DestroyItemImmediate();
      }
      EditorUtils.PopColor();
      EditorGUILayout.Space();
      EditorUtils.PushColor();

      foreach (var prefab in Target.Preset.Items)
      {
        EditorUtils.PushColor();
        if (Target.Prefab != null && prefab == Target.Prefab) GUI.color = Color.green;
        if (GUILayout.Button(prefab.gameObject.name, EditorStyles.miniButton))
        {
          Target.DestroyItemImmediate();
          Target.CreateItem(prefab);
          Target.SetItemPosition();
          Target.AttachItem();
        }
        EditorUtils.PopColor();
      }
      EditorUtils.PopColor();
    }
  }
}
