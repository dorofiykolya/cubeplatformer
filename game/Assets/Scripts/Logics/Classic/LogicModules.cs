using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Logics.Classics
{
  public class LogicModules
  {
    private readonly List<ILogicModule> _modules = new List<ILogicModule>();

    public LogicModules()
    {
      _modules.Add(new LogicUserInputModule(this));
      _modules.Add(new LogicRunnerModule(this));
      _modules.Add(new LogicGuardModule(this));
      _modules.Add(new LogicHoleModule(this));
      _modules.Add(new LogicMapModule(this));
      _modules.Add(new LogicViewModule(this));
    }

    public T Get<T>() where T : class, ILogicModule
    {
      return _modules.FirstOrDefault(m => m is T) as T;
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