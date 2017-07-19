using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
  [CustomEditor(typeof(GameStartBehaviour), true)]
  public class GameStartBehaviourEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      if (Application.isPlaying)
      {
        if (GUILayout.Button("Restart"))
        {
          ((GameStartBehaviour)target).Restart();
        }
      }
    }
  }
}