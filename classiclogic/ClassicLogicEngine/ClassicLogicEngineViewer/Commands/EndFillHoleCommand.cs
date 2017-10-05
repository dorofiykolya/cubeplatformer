using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class EndFillHoleCommand : PlayCommand<EndFillHoleEvent>
  {
    protected override void Execute(EndFillHoleEvent value, TileContainer tileContainer)
    {
      tileContainer.EndFillHole(value.X, value.Y);
    }
  }
}
