using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Logics.Classics
{
  public class LogicModules
  {
    private readonly Queue<ILogicModule> _modules = new Queue<ILogicModule>();

    public LogicModules()
    {
      _modules.Enqueue(new LogicUserInputModule());
      _modules.Enqueue(new LogicRunnerModule());
      _modules.Enqueue(new LogicGuardModule());
      _modules.Enqueue(new LogicHoleModule());
    }

    public void PreTick(LogicEngine logicEngine, int currentTick, int deltaTick)
    {
      foreach (var module in _modules)
      {
        module.PreTick(logicEngine, currentTick, deltaTick);
      }
    }

    public void Tick(LogicEngine logicEngine, int currentTick, int deltaTick)
    {
      foreach (var module in _modules)
      {
        module.Tick(logicEngine, currentTick, deltaTick);
      }
    }

    public void PostTick(LogicEngine logicEngine, int currentTick, int deltaTick)
    {
      foreach (var module in _modules)
      {
        module.PostTick(logicEngine, currentTick, deltaTick);
      }
    }
  }
}