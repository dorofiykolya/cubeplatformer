namespace Game.Logics
{
  public interface ILogicEngine
  {
    void AddAction(ILogicAction action);
    void FastForward(int tick);
    int Tick { get; }
    bool IsFinished { get; }
    int MaxTicks { get; }
  }
}