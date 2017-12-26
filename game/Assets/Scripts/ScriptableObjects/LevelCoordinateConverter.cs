using UnityEngine;

namespace Game
{
  public abstract class LevelCoordinateConverter : ScriptableObject, ILevelCoordinateConverter
  {
    public abstract Vector3 ToWorld(PositionF logicPosition);
    public abstract Position ToPosition(Vector3 worldPosition);
    public abstract PositionF ToPositionF(Vector3 worldPosition);
  }
}