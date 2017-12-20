using UnityEngine;

namespace Game
{
  public class UnitLevelCoordinateConverter : LevelCoordinateConverter
  {
    public Vector3 UnitSize = Vector3.one;

    public override Vector3 ToWorld(PositionF logicPosition)
    {
      return new Vector3(logicPosition.X * UnitSize.x, logicPosition.Y * UnitSize.y, logicPosition.Z * UnitSize.z);
    }

    public override PositionF ToPositionF(Vector3 worldPosition)
    {
      return new PositionF(worldPosition.x / UnitSize.x, worldPosition.y / UnitSize.y, worldPosition.z / UnitSize.z);
    }

    public override Position ToPosition(Vector3 worldPosition)
    {
      return new Position((int)(worldPosition.x / UnitSize.x), (int)(worldPosition.y / UnitSize.y), (int)(worldPosition.z / UnitSize.z));
    }
  }
}