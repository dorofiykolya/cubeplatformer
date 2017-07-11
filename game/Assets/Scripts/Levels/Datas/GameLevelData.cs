using System;
using Game.Views.Components;

namespace Game
{
  [Serializable]
  public class GameLevelData
  {
    public string Name;
    public string Description;
    public LevelComponent LevelPrefab;
    public EnvironmentComponent EnvironmentPrefab;
    public string Scene;
  }
}