using Game.Components;
using Game.Levels;
using UnityEngine;
using Utils;

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

      if (options != null)
      {
        if (options.Auto2DBoundCollider) Auto2DBoundCollider(level);
      }

      return level;
    }

    private static void Auto2DBoundCollider(LevelComponent level)
    {
      var colliders = new GameObject("2DColliders").transform;
      colliders.SetParent(level.transform, false);

      var right = new GameObject("Right2DCollider").AddComponent<BoxCollider2D>();
      right.size = level.CoordinateConverter.ToWorld(new PositionF(10, level.Size.Y + 4, 0));
      right.transform.localPosition = level.CoordinateConverter.ToWorld(new PositionF(level.Size.X, level.Size.Y / 2f, 0)) + new Vector3(right.size.x / 2, 0, 0);
      right.transform.SetParent(colliders, false);


      var left = new GameObject("Left2DCollider").AddComponent<BoxCollider2D>();
      left.size = level.CoordinateConverter.ToWorld(new PositionF(10, level.Size.Y + 4, 0));
      left.transform.localPosition = level.CoordinateConverter.ToWorld(new PositionF(0, level.Size.Y / 2f, 0)) - new Vector3(left.size.x / 2, 0, 0);
      left.transform.SetParent(colliders, false);

      var top = new GameObject("Top2DCollider").AddComponent<BoxCollider2D>();
      top.size = level.CoordinateConverter.ToWorld(new PositionF(level.Size.X + 4, 10, 0));
      top.transform.localPosition = level.CoordinateConverter.ToWorld(new PositionF(level.Size.X / 2f, level.Size.Y, 0)) + new Vector3(0, top.size.y / 2f, 0);
      top.transform.SetParent(colliders, false);

      var bottom = new GameObject("Bottom2DCollider").AddComponent<BoxCollider2D>();
      bottom.size = level.CoordinateConverter.ToWorld(new PositionF(level.Size.X + 4, 10, 0));
      bottom.transform.localPosition = level[level.Size.X / 2, 0, 0].transform.localPosition.SetY(bottom.size.y / -2f);
      bottom.transform.SetParent(colliders, false);
    }
  }
}