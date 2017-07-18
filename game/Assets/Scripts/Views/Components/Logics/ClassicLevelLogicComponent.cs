using Game.Logics;
using UnityEngine;

namespace Game.Views.Components
{
  [RequireComponent(typeof(LevelComponent))]
  public class ClassicLevelLogicComponent : LevelLogicComponent
  {
    private ILogicEngine _engine;

    public override ILogicEngine Engine
    {
      get { return _engine ?? (_engine = CreateEngine()); }
    }

    private ILogicEngine CreateEngine()
    {
      var engine = new Logics.Classics.LogicEngine();
      var level = GetComponent<LevelComponent>();
      engine.SetGrid(level.Size, level.Grid);

      return engine;
    }
  }
}