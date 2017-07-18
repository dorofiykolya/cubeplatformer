using UnityEngine;
using System.Collections;

namespace Game.Inputs
{
  public struct InputEvent
  {
    public GameInput Input;
    public float Value;
    public InputStatus Status;
    public int ControllerId;
  }
}