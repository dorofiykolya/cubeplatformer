using UnityEngine;
using System.Collections;

namespace Game.Logics
{
  public class LogicActionRemovePlayer : LogicAction
  {
    public int PlayerId;

    public LogicActionRemovePlayer(int playerId, int tick)
    {
      PlayerId = playerId;
      Tick = tick;
    }
  }
}