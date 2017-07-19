using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicCoinModule : ILogicModule
  {
    private readonly LogicModules _logicModules;
    public int goldCount;
    public bool goldComplete;

    public LogicCoinModule(LogicModules logicModules)
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

    public void AddGold(int x, int y)
    {
      
    }

    public void RemoveGold(int x, int y)
    {
      
    }

    public void DecGold()
    {
      
    }
  }
}