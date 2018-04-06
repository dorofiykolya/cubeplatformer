using System;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class GameLevelData : ScriptableObject
  {
    public string Name;
    public string Description;
    public GameSubLevelData[] Levels;

    public GameSubLevelData GetLevel(int subLevel)
    {
      return Levels[subLevel];
    }
  }
}