using System.Collections.Generic;
using System.Text;
using ClassicLogic.Engine;
using ClassicLogic.Utils;
using Game.Logics.Actions;
using Game.Views.Components;
using UnityEngine;
using InputAction = ClassicLogic.Engine.InputAction;

namespace Game.Logics.ClassicLogic
{
  public class ClassicLogicEngine : ILogicEngine
  {
    private readonly Dictionary<int, CellGuardContentComponent> _guards = new Dictionary<int, CellGuardContentComponent>();
    private readonly ClassicLogicCommandProcessor _processor = new ClassicLogicCommandProcessor();
    private readonly LevelComponent _level;
    private readonly Engine _engine;
    private readonly Transform _guardTransform;

    public ClassicLogicEngine(LevelComponent level)
    {
      _level = level;

      var converter = level.CoordinateConverter as ClassicLogicLevelCoordinateConverter;
      converter.Size = level.Size;

      var data = ClassicLogicLevelConverter.Convert(level);

      _guardTransform = new GameObject("Guards").transform;
      _guardTransform.SetParent(level.transform, false);

      _engine = new Engine(AIVersion.V4, new StringLevelReader(data), Mode.Modern);
    }

    public void SetAction(Actions.InputAction action)
    {
      _engine.SetAction((InputAction)(int)action);
    }

    public EngineOutput Output
    {
      get { return _engine.Output; }
    }

    public void AddAction(ILogicAction action)
    {
      var input = action as LogicActionInputAction;
      if (input != null)
      {
        _engine.SetAction((InputAction)(int)input.InputAction);
      }
    }

    public void FastForward(int tick)
    {

      _engine.FastForward(tick);
      while (_engine.Output.Count != 0)
      {
        var evt = _engine.Output.Dequeue();
        _processor.Execute(evt, this);
        _engine.Output.Return(evt);
      }
    }

    public int Tick { get { return _engine.State.Tick; } }
    public bool IsFinished { get { return _engine.State.State == global::ClassicLogic.Engine.GameState.GAME_FINISH; } }
    public int MaxTicks { get { return int.MaxValue; } }
    public int TicksPerSeconds { get { return 10; } }

    public Transform GuardTransform
    {
      get { return _guardTransform; }
    }

    public LevelComponent Level
    {
      get { return _level; }
    }

    public CellPlayerContentComponent Runner { get; set; }
    public CellContentComponent HideLadder { get; set; }

    public void AddGuard(int guardDataId, CellGuardContentComponent view)
    {
      _guards[guardDataId] = view;
    }

    public CellGuardContentComponent GetGuard(int guardId)
    {
      return _guards[guardId];
    }
  }
}
