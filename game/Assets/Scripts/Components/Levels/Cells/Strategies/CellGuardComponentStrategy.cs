using UnityEngine;

namespace Game.Components
{
  public class CellGuardComponentStrategy
  {
    public static void Process(CellComponent cellComponent, CellType cellType)
    {
      var guard = cellComponent.GetComponent<CellGuardComponent>();
      if (guard != null && cellType != CellType.Guard)
      {
        if (Application.isEditor) GameObject.DestroyImmediate(guard);
        else GameObject.Destroy(guard);
      }
      else if (guard == null && cellType == CellType.Guard)
      {
        cellComponent.gameObject.AddComponent<CellGuardComponent>();
      }
    }
  }
}