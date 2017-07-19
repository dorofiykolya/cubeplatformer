using UnityEngine;
using System.Collections;
using Game.Logics.Classics;

namespace Game.Logics.Classics
{
  public class LogicHole : LogicActor
  {
    private int _digTicks;
    public LogicActorAction Action;
    public int PlayerId;

    public bool IsDigging { get { return _digTicks > 0; } }

    public void StartDigging(LogicCell cell)
    {
      _digTicks = 60;
    }

    public void Fill()
    {

    }

    public bool Tick()
    {
      var isDig = IsDigging;
      _digTicks = Mathf.Max(--_digTicks, 0);
      return isDig && !IsDigging;
    }
  }
}