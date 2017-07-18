using System;

namespace Game.Logics.Classics
{
    public class LogicViewModule : ILogicModule
    {
        private LogicModules logicModules;

        public LogicViewModule(LogicModules logicModules)
        {
            this.logicModules = logicModules;
        }

        public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
        {
            throw new NotImplementedException();
        }

        public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
        {
            throw new NotImplementedException();
        }

        public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
        {
            throw new NotImplementedException();
        }
    }
}