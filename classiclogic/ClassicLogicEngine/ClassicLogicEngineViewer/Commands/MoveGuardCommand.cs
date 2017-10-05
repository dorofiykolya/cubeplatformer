using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class MoveGuardCommand : PlayCommand<MoveGuardEvent>
  {
    protected override void Execute(MoveGuardEvent value, TileContainer tileContainer)
    {
      tileContainer.MoveGuard(value.Id, value.X, value.Y);
    }
  }
}
