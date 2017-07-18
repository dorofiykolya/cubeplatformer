using UnityEngine;
using System.Collections;
using Utils;

namespace Game.Inputs
{
  public class LevelInputContenxt : InputContext
  {
    public LevelInputContenxt(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
    {
    }
  }
}