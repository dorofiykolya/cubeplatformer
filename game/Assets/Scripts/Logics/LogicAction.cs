using System.Collections.Generic;

namespace Game.Logics
{
  public abstract class LogicAction : ILogicAction
  {
    private static readonly Comparer<int> Comparer = Comparer<int>.Default;

    public int CompareTo(ILogicAction other)
    {
      return Comparer.Compare(Tick, other.Tick);
    }

    public int CompareTo(object obj)
    {
      var other = obj as ILogicAction;
      if (other != null)
      {
        return CompareTo(other);
      }
      return -1;
    }

    public int Tick { get; protected set; }
  }
}