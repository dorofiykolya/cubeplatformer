using UnityEngine;

namespace Game
{
  public abstract class LevelCoordinateConverter : ScriptableObject, ILevelCoordinateConverter
  {
    public abstract Vector3 ToWorld(Position logicPosition);
    public abstract Position ToPosition(Vector3 worldPosition);
  }
}