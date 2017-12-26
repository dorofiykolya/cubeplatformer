using Game.Components;
using Game.Levels;
using UnityEngine;

namespace Game
{
  public class LevelFactory
  {
    public static LevelComponent Create(LevelSize size, LevelCoordinateConverter coordinateConverter, LevelFactoryOptions options = null)
    {
      var gameObject = new GameObject("Level");
      gameObject.AddComponent<LevelControllerComponent>();
      var level = gameObject.AddComponent<LevelComponent>();
      gameObject.AddComponent<LevelCoordinateConverterProviderComponent>().SetConverter(coordinateConverter);
      gameObject.AddComponent<LevelGizmoComponent>();
      gameObject.AddComponent<LevelLogicComponent>();

      level.SetContent(size, LevelCellFactory.Create(size, level));

      return level;
    }
  }
}