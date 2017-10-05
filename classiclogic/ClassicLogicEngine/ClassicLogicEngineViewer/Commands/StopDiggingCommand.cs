using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class StopDiggingCommand : PlayCommand<StopDiggingEvent>
  {
    protected override void Execute(StopDiggingEvent value, TileContainer tileContainer)
    {
      tileContainer.StopDigging();
    }
  }
}
