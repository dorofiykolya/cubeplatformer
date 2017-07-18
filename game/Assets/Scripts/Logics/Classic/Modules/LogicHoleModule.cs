using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicHoleModule : ILogicModule
  {
        private LogicModules logicModules;

        public LogicHoleModule(LogicModules logicModules)
        {
            this.logicModules = logicModules;
        }

        public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      throw new System.NotImplementedException();
    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      throw new System.NotImplementedException();
    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      throw new System.NotImplementedException();
    }
  }
}