using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class MoveRunnerCommand : PlayCommand<MoveRunnerEvent>
  {
    protected override void Execute(MoveRunnerEvent value, TileContainer tileContainer)
    {
      tileContainer.MoveRunner(value.x, value.y);
    }
  }
}
