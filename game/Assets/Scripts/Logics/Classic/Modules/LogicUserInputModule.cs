using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicUserInputModule : ILogicModule
  {
    private LogicModules _logicModules;
    public LogicActorAction KeyAction;

    public LogicUserInputModule(LogicModules logicModules)
    {
      _logicModules = logicModules;
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