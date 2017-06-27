using UnityEngine;

namespace Game
{
  public interface ILevelCoordinateConverter
  {
    Vector3 ToWorld(Position logicPosition);
    Position ToPosition(Vector3 worldPosition);
  }
}