using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class StartFillHoleCommand : PlayCommand<StartFillHoleEvent>
  {
    protected override void Execute(StartFillHoleEvent value, TileContainer tileContainer)
    {
      tileContainer.StartFillHole(value.X, value.Y);
    }
  }
}
