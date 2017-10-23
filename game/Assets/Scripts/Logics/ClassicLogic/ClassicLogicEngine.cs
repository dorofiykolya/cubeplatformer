using ClassicLogic.Engine;
using ClassicLogic.Utils;
using Game.Logics.Actions;
using UnityEngine;
using Utils;
using InputAction = ClassicLogic.Engine.InputAction;

namespace Game.Logics.Classic
{
  public class ClassicLogicEngine : ILogicEngine
  {
    private readonly ClassicLogicCommandProcessor _processor = new ClassicLogicCommandProcessor();
    private readonly Engine _engine;

    private readonly ClassicLogicViewContext _viewContext;

    public ClassicLogicEngine(Lifetime lifetime, string data)
    {
      data = data.Replace("\r\n", "\n");
      _engine = new Engine(new StringLevelReader(data));
      _viewContext = new ClassicLogicViewContext(lifetime, _engine.State.MaxTileX, _engine.State.MaxTileY);
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
        Debug.Log((InputAction)(int)input.InputAction);
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
    public bool IsFinished { get { return _engine.State.State == ClassicLogic.Engine.GameState.GameFinish; } }
    public int MaxTicks { get { return int.MaxValue; } }
    public int TicksPerSeconds { get { return 30; } }

    public ClassicLogicViewContext ViewContext { get { return _viewContext; } }
  }
}
