using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(CellComponent))]
  public class CellGuardComponent : MonoBehaviour
  {
    public float DelaySpawnSeconds = 1f;
  }
}