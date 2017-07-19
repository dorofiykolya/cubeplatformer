using UnityEngine;
using System.Collections;
using System.Linq;
using Game.Inputs;
using Game.Logics;

namespace Game.Logics
{
  public class LogicActionInitializePlayer : LogicAction
  {
    public int[] Players;

    public LogicActionInitializePlayer(InputController[] inputControllers, int tick)
    {
      Tick = tick;
      Players = inputControllers.Select(p => p.Id).ToArray();
    }
  }
}