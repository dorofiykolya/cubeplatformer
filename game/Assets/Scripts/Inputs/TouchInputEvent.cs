using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Inputs
{
  public struct TouchInputEvent
  {
    public int Id;
    public Vector2 Position;
    public TouchPhase Phase;
    public Vector2 DeltaPosition;
  }
}
