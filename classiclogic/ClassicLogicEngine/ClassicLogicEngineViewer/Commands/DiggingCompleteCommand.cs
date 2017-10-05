using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class DiggingCompleteCommand : PlayCommand<DiggingCompleteEvent>
  {
    protected override void Execute(DiggingCompleteEvent value, TileContainer tileContainer)
    {
      tileContainer.DiggingComplete();
    }
  }
}
