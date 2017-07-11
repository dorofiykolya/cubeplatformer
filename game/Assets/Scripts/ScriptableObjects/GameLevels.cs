using System;
using UnityEngine;
using System.Collections;

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