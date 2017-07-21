using UnityEngine;

namespace Game
{
  public interface ILevelCoordinateConverter
  {
    Vector3 ToWorld(PositionF logicPosition);
    PositionF ToPosition(Vector3 worldPosition);
  }
}