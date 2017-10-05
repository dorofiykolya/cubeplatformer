using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class RemoveGoldCommand : PlayCommand<RemoveGoldEvent>
  {
    protected override void Execute(RemoveGoldEvent value, TileContainer tileContainer)
    {
      tileContainer.RemoveGold(value.X, value.Y);
    }
  }
}
