using UnityEngine;

namespace Game
{
  public class DefaultLevelCoordinateConverter : LevelCoordinateConverter
  {
    public override Vector3 ToWorld(PositionF logicPosition)
    {
      return new Vector3(logicPosition.X, logicPosition.Y, logicPosition.Z);
    }

    public override PositionF ToPosition(Vector3 worldPosition)
    {
      return new Position((int)worldPosition.x, (int)worldPosition.y, (int)worldPosition.z);
    }
  }
}