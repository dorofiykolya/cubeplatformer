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
  public class ShowHideLadderCommand : PlayCommand<ShowHideLadderEvent>
  {
    protected override void Execute(ShowHideLadderEvent value, TileContainer tileContainer)
    {
      foreach (var tile in tileContainer.Tiles)
      {
        if (tile.Type == TileType.HLADR_T)
        {
          tile.Visibility = Visibility.Visible;
        }
      }
    }
  }
}
