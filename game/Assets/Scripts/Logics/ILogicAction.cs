using System;

namespace Game.Logics
{
  public interface ILogicAction : IComparable<ILogicAction>
  {
    int Tick { get; }
  }
}