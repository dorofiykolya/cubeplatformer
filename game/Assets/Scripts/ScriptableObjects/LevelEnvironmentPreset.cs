using System;
using Game.Components.Levels;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class LevelEnvironmentPreset : ScriptableObject
  {
    public LevelEnvironmentItemComponent[] Items;
    public LevelEnvironmentItemComponent[] BottomEdgeItems;
    public LevelEnvironmentItemComponent[] LeftEdgeItems;
    public LevelEnvironmentItemComponent[] RightEdgeItems;
    public LevelEnvironmentItemComponent[] TopEdgeItems;
  }
}
