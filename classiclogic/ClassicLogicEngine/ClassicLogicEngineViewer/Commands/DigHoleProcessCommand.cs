using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class DigHoleProcessCommand : PlayCommand<DigHoleProcessEvent>
  {
    protected override void Execute(DigHoleProcessEvent value, TileContainer tileContainer)
    {
      tileContainer.ProcessDigging(value.Ratio);
    }
  }
}
