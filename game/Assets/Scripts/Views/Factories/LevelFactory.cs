using Game.Views.Components;
using UnityEngine;

namespace Game.Views
{
  public class LevelFactory
  {
    public static LevelComponent Create(LevelSize size)
    {
      var gameObject = new GameObject("Level");
      var level = gameObject.AddComponent<LevelComponent>();

      gameObject.AddComponent<LevelCoordinateConverterProviderComponent>();
      gameObject.AddComponent<LevelGizmoComponent>();

      level.SetContent(size, LevelCellFactory.Create(size, level));

      return level;
    }
  }
}