using UnityEngine;

namespace Game.Components
{
  public class CellCoinComponentStrategy
  {
    public static void Process(CellComponent cellComponent, CellType cellType)
    {
      var coin = cellComponent.GetComponent<CellCoinComponent>();
      if (coin != null && cellType != CellType.Coin)
      {
        if (Application.isEditor) GameObject.DestroyImmediate(coin);
        else GameObject.Destroy(coin);
      }
      else if (coin == null && cellType == CellType.Coin)
      {
        cellComponent.gameObject.AddComponent<CellCoinComponent>();
      }
    }
  }
}