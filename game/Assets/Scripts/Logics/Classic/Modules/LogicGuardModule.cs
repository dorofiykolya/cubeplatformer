using UnityEngine;
using System.Collections;
using Game.Views.Components;
using System;

namespace Game.Logics.Classics
{
  public class LogicGuardModule : ILogicModule
  {
    private LogicModules _logicModules;

    public LogicGuardModule(LogicModules logicModules)
    {
      this._logicModules = logicModules;
    }

    public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    internal void AddGuard(CellComponent cell)
    {

    }
  }
}