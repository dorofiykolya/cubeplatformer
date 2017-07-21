using System;
using Game.Views.Components;

namespace Game.Logics.Classics
{
  public class LogicEngine : ILogicEngine
  {
    private LogicActions _actionsQueue = new LogicActions();
    private LogicModules _modules = new LogicModules();
    private LogicProcessor _processor = new LogicProcessor();

    public LogicModules Modules { get { return _modules; } }
    public LogicActions Actions { get { return _actionsQueue; } }

    public void AddAction(ILogicAction action)
    {
      _actionsQueue.Enqueue(action);
    }

    public void FastForward(int tick)
    {
      if (!IsFinished)
      {
        var finish = false;
        if (tick >= MaxTicks)
        {
          tick = MaxTicks;
          finish = true;
        }
        var currentTick = Tick;
        while (++currentTick <= tick)
        {
          var deltaTick = UpdateTick(currentTick);
          _modules.PreTick(this, currentTick, deltaTick);
          ILogicAction currentAction;
          if (!IsFinished && _actionsQueue.Count > 0 && (currentAction = _actionsQueue.Peek()).Tick < currentTick)
          {
            _actionsQueue.Dequeue();
            _processor.Execute(currentAction, this);
          }
          _modules.Tick(this, currentTick, deltaTick);
          _modules.PostTick(this, currentTick, deltaTick);
          if (IsFinished)
          {
            break;
          }
        }
        if (!IsFinished)
        {
          UpdateTick(tick);
          if (finish)
          {
            Finish();
          }
        }
      }
    }

    public void InitializeLevel(GameContext context, LevelSize size, CellComponent[] grid, LevelComponent level, ILevelCoordinateConverter levelCoordinateConverter)
    {
      _modules.Get<LogicViewModule>().Initialize(context, levelCoordinateConverter, level);
      _modules.Get<LogicGridModule>().Initialize(size, grid);
    }

    public int Tick { get; private set; }
    public bool IsFinished { get; private set; }
    public int MaxTicks { get { return int.MaxValue; } }

    public void Finish()
    {
      IsFinished = true;
    }

    protected int UpdateTick(int tick)
    {
      var current = Tick;
      var delta = tick - current;
      Tick = tick;
      return delta;
    }

    public int TicksPerSeconds { get { return 60; } }
  }
}