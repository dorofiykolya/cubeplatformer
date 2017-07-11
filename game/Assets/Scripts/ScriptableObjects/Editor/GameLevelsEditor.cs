using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
  [CustomEditor(typeof(GameLevels))]
  public class GameLevelsEditor : Editor<GameLevels>
  {
    public override void OnInspectorGUI()
    {
      var levels = Target.Levels;
      for (int i = 0; i < levels.Length; i++)
      {
        var level = levels[i];
        if (level == null)
        {
          level = new GameLevelData
          {
            Name = "None"
          };
          levels[i] = level;
          EditorUtility.SetDirty(Target);
        }
      }
      base.OnInspectorGUI();
    }
  }
}