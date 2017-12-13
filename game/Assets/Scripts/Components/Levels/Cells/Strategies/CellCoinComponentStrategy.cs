using UnityEngine;

namespace Game.Views.Components
{
  public class CellCoinComponentStrategy
  {
    public static void Process(CellComponent cellComponent, CellType cellType)
    {
      var coin = cellComponent.GetComponent<CellCoinComponent>();
      if (coin != null && cellType != CellType.Gold)
      {
        if (Application.isEditor) GameObject.DestroyImmediate(coin);
        else GameObject.Destroy(coin);
      }
      else if (coin == null && cellType == CellType.Gold)
      {
        cellComponent.gameObject.AddComponent<CellCoinComponent>();
      }
    }
  }
}