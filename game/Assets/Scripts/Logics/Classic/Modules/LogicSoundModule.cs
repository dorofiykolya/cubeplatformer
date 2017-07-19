using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicSoundModule : ILogicModule
  {
    private readonly LogicModules _logicModules;

    public LogicSoundModule(LogicModules logicModules)
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

    public void PlayTrap()
    {
      
    }

    public void PlayDig()
    {
      
    }

    public void PlayDown()
    {
      
    }

    public void StopFall()
    {
      
    }

    public void PlayFall()
    {
      
    }

    public void PlayGetGold()
    {
      
    }

    public void StopDig()
    {
      
    }
  }
}