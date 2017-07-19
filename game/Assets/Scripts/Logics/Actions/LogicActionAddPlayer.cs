using UnityEngine;
using System.Collections;

namespace Game.Logics
{
  public class LogicActionAddPlayer : LogicAction
  {
    public int PlayerId;

    public LogicActionAddPlayer(int playerId, int tick)
    {
      PlayerId = playerId;
      Tick = tick;
    }
  }
}