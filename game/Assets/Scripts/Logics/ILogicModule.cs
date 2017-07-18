namespace Game.Logics
{
  public interface ILogicModule
  {
    void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick);
    void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick);
    void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick);
  }
}