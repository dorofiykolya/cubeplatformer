using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClassicLogic.Engine;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  class InitializeCommand : PlayCommand<InitializeEvent>
  {
    protected override void Execute(InitializeEvent value, TileContainer container)
    {
      for (int x = 0; x < value.Map.Length; x++)
      {
        for (int y = 0; y < value.Map[0].Length; y++)
        {
          var type = value.Map[x][y];
          var tile = container.AddTile(x, y, type);
          if (type == TileType.HLADR_T)
          {
            tile.Visibility = Visibility.Hidden;
          }
          else if (type == TileType.TRAP_T)
          {
            tile.Visibility = Visibility.Hidden;
          }
        }
      }
      foreach (var guardData in value.Guard)
      {
        container.AddGuard(guardData.Position, guardData.Id);
      }
      container.AddRunner(value.Runner);
    }
  }
}
