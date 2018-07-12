using UnityEditor;
using UnityEngine;

namespace Game.Components.Levels.Editor
{
  [CustomEditor(typeof(LevelEnvironmentComponent))]
  public class LevelEnviromentComponentEditor : Editor<LevelEnvironmentComponent>
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      if (GUILayout.Button("Build"))
      {
        Target.Build();
      }
    }
  }
}
