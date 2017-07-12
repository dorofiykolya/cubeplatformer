using System;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class GameLevels : ScriptableObject
  {
    [SerializeField]
    public GameLevelData[] Levels;

    public GameLevelData GetLevel(int index)
    {
      return Levels[index];
    }
  }
}