using System;
namespace ClassicLogic.Engine
{
  public enum Action
  {
    ACT_UNKNOWN = -1,
    ACT_STOP = 0,
    ACT_LEFT = 1,
    ACT_RIGHT = 2,
    ACT_UP = 3,
    ACT_DOWN = 4,
    ACT_FALL = 5,
    ACT_FALL_BAR = 6,
    ACT_DIG_LEFT = 7,
    ACT_DIG_RIGHT = 8,
    ACT_DIGGING = 9,
    ACT_IN_HOLE = 10,
    ACT_CLIMB_OUT = 11,
    ACT_REBORN = 12;
  }
}
