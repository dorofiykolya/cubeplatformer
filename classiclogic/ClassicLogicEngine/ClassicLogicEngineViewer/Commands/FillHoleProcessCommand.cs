using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class FillHoleProcessCommand : PlayCommand<FillHoleProcessEvent>
  {
    protected override void Execute(FillHoleProcessEvent value, TileContainer tileContainer)
    {
      tileContainer.ProcessFillHole(value.X, value.Y, value.Ratio);
    }
  }
}
