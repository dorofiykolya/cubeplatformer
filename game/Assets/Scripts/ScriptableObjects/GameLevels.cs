using System;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class GameLevels : ScriptableObject
  {
    public GameLevelData[] Levels;

    public GameLevelData GetLevel(int index)
    {
      if (index >= 0 && index < Levels.Length)
      {
        return Levels[index];
      }

      return null;
    }
  }
}