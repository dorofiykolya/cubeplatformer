using System;
using UnityEngine;

namespace Game.Logics.ClassicLogic
{
  [Serializable]
  public class ClassicLogicLevelCoordinateConverter : LevelCoordinateConverter
  {
    public LevelSize Size;

    public override Vector3 ToWorld(PositionF logicPosition)
    {
      return new Vector3(logicPosition.X, Size.Y - 1 - logicPosition.Y, logicPosition.Z);
    }

    public override PositionF ToPosition(Vector3 worldPosition)
    {
      return new Position((int)worldPosition.x, Size.Y - 1 - (int)worldPosition.y, (int)worldPosition.z);
    }
  }
}
