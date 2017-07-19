using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicGuard : LogicActor
  {
    public LogicActorAction Action;
    public int HasGold;
    public LogicActorAction LastAction;
    public float XOffset;
    public float YOffset;
    public Position SpawnPosition;
    public LogicActorShape shape;
    public int Id;

    public LogicGuard(Position position)
    {
      SpawnPosition = position;
      Position = position;
    }

    public void SetOffset(float xOffset, float yOffset)
    {
      XOffset = xOffset;
      YOffset = yOffset;
    }
  }
}