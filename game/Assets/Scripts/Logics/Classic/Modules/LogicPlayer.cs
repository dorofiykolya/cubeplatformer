using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicPlayer : LogicActor
  {
    public float yMove = 0.08f;
    public float xMove = 0.08f;
    public LogicActorAction Action;
    public float xOffset;
    public float yOffset;
    public LogicActorAction shape;
    public int Id;
    public LogicActorAction LastAction;

    public void SetOffset(float xoffset, float yoffset)
    {
      xOffset = xoffset;
      yOffset = yoffset;
    }
  }
}