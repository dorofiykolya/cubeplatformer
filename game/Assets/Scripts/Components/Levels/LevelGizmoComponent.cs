using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(LevelComponent)), ExecuteInEditMode]
  public class LevelGizmoComponent : MonoBehaviour
  {
    public bool ShowGizmosSelectedGrid = true;
    public bool ShowGizmosWireGrid = true;
    public bool ShowGizmosGrid = true;
    public bool ShowGizmoEnviroment = true;

    public Vector3 UnitSize
    {
      get { return GetComponent<LevelComponent>().CoordinateConverter.ToWorld(new PositionF(1, 1, 1)); }
    }
  }
}