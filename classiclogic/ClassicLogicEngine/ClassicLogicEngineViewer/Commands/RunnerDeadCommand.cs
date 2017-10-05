using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class RunnerDeadCommand : PlayCommand<RunnerDeadEvent>
  {
    protected override void Execute(RunnerDeadEvent value, TileContainer tileContainer)
    {
      tileContainer.RunnerDead();
    }
  }
}
