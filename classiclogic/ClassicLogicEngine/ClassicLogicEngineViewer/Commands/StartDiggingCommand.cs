using System;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class StartDiggingCommand : PlayCommand<StartDiggingEvent>
  {
    protected override void Execute(StartDiggingEvent value, TileContainer tileContainer)
    {
      tileContainer.StartDigging(value.X, value.Y);
    }
  }
}
