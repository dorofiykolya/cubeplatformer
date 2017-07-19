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
      _modules.Add(new LogicCoinModule(this));
      _modules.Add(new LogicPlayerModule(this));
      _modules.Add(new LogicGuardModule(this));
      _modules.Add(new LogicHoleModule(this));
      _modules.Add(new LogicGridModule(this));
      _modules.Add(new LogicViewModule(this));
      _modules.Add(new LogicSoundModule(this));
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