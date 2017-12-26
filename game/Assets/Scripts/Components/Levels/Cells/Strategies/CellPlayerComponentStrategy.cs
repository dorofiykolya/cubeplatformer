using UnityEngine;

namespace Game.Components
{
    public class CellPlayerComponentStrategy
    {
        public static void Process(CellComponent cellComponent, CellType cellType)
        {
            var player = cellComponent.GetComponent<CellPlayerComponent>();
            if (player != null && cellType != CellType.Player)
            {
                if (Application.isEditor) GameObject.DestroyImmediate(player);
                else GameObject.Destroy(player);
            }
            else if (player == null && cellType == CellType.Player)
            {
                cellComponent.gameObject.AddComponent<CellPlayerComponent>();
            }
        }
    }
}