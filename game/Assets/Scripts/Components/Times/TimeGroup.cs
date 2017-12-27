using System;

namespace Game.Components
{
  [Flags]
  public enum TimeGroup
  {
    All = Player | Guard | Environment,
    Player = 1,
    Guard = 1 << 1,
    Environment = 1 << 2,
  }
}
