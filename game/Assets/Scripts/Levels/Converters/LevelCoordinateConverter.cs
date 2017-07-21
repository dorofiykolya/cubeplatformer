using UnityEngine;

namespace Game
{
  public abstract class LevelCoordinateConverter : ScriptableObject, ILevelCoordinateConverter
  {
    public abstract Vector3 ToWorld(PositionF logicPosition);
    public abstract PositionF ToPosition(Vector3 worldPosition);
  }
}