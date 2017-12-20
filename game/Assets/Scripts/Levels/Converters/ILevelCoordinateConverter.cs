using UnityEngine;

namespace Game
{
  public interface ILevelCoordinateConverter
  {
    Vector3 ToWorld(PositionF logicPosition);
    Position ToPosition(Vector3 worldPosition);
    PositionF ToPositionF(Vector3 worldPosition);
  }
}