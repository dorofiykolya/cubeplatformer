using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class ShowTrapCommand : PlayCommand<ShowTrapEvent>
  {
    protected override void Execute(ShowTrapEvent value, TileContainer tileContainer)
    {
      tileContainer.ShowTrap(value.X, value.Y);
    }
  }
}
