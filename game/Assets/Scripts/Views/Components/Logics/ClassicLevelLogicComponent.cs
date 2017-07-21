using Game.Logics;
using UnityEngine;

namespace Game.Views.Components
{
  [RequireComponent(typeof(LevelComponent))]
  public class ClassicLevelLogicComponent : LevelLogicComponent
  {
    private ILogicEngine _engine;

    public override ILogicEngine Engine(GameContext context)
    {
      return _engine ?? (_engine = CreateEngine(context));
    }

    private ILogicEngine CreateEngine(GameContext context)
    {
      var engine = new Logics.Classics.LogicEngine();
      var level = GetComponent<LevelComponent>();
      engine.InitializeLevel(context, level.Size, level.Grid, level, level.CoordinateConverter);

      return engine;
    }
  }
}