using UnityEngine;

namespace Game.Views
{
  public class LevelCellFactory
  {
    public static CellComponent[] Create(LevelSize size, LevelComponent level)
    {
      DeleteGrid(level);

      if (size.Z == 0) size.Z = 1;

      var count = size.X * size.Y * size.Z;
      var result = new CellComponent[count];
      var index = 0;
      for (int x = 0; x < size.X; x++)
      {
        for (int y = 0; y < size.Y; y++)
        {
          for (int z = 0; z < size.Z; z++)
          {
            var gameObject = new GameObject(string.Format("x:{0}, y:{1}, z:{2}", x, y, z));
            var cellComponent = gameObject.AddComponent<CellComponent>();
            cellComponent.Level = level;
            cellComponent.Position = new Position(x, y, z);
            gameObject.AddComponent<CellGizmoComponent>();
            
            result[index] = cellComponent;
            index++;
          }
        }
      }
      return result;
    }

    private static void DeleteGrid(LevelComponent level)
    {
      if (level.Grid != null)
      {
        foreach (var cellComponent in level.Grid)
        {
          Object.DestroyImmediate(cellComponent.gameObject);
        }
      }
    }
  }
}
