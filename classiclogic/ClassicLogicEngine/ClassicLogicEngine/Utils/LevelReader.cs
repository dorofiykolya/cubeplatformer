namespace ClassicLogic.Utils
{
  public abstract class LevelReader
  {
    public abstract bool MoveNext();
    public abstract LevelToken Token { get; }
    public abstract void Reset();
  }
}
