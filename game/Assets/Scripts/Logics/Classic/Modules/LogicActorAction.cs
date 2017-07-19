using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public enum LogicActorAction
  {
    Unknown = -1,
    Stop = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
    Fall = 5,
    FallBar = 6,
    DigLeft = 7,
    DigRight = 8,
    Digging = 9,
    InHole = 10,
    ClimpOut = 11,
    Reborn = 12
  }
}