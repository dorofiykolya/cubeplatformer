using UnityEngine;

namespace Game.Views.Components
{
  [RequireComponent(typeof(CellComponent))]
  public class CellGuardComponent : MonoBehaviour
  {
    public float DelaySpawnSeconds = 1f;
  }
}