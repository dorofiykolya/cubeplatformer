using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicUserInputModule : ILogicModule
  {
    private LogicModules logicModules;

    public LogicUserInputModule(LogicModules logicModules)
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
  }
}