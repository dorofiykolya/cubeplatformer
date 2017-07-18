using UnityEngine;
using System.Collections;
using Game.Views.Components;
using System;

namespace Game.Logics.Classics
{
  public class LogicRunnerModule : ILogicModule
  {
    private LogicModules logicModules;

    public LogicRunnerModule(LogicModules logicModules)
    {
      this.logicModules = logicModules;
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

    public void AddRunner(CellComponent cell)
    {
      
    }
  }
}