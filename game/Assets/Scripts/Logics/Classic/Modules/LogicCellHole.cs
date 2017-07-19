using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicCellHole
  {
    private int _ticks;

    public Position Position;
    public int PlayerId;
    public bool IsFilled { get { return _ticks > 0; } }

    public void Start()
    {
      _ticks = 60 * 3;
    }

    public bool Tick()
    {
      var filled = IsFilled;
      _ticks = Mathf.Max(--_ticks, 0);
      return filled && !IsFilled;
    }
  }
}