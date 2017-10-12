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
      var level = GetComponent<LevelComponent>();
      var engine = new Game.Logics.ClassicLogic.ClassicLogicEngine(level);
      return engine;
    }
  }
}