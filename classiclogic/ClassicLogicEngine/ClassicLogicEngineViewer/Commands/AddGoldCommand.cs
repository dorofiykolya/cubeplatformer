using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class AddGoldCommand : PlayCommand<AddGoldEvent>
  {
    protected override void Execute(AddGoldEvent value, TileContainer tileContainer)
    {
      tileContainer.AddGold(value.X, value.Y);
    }
  }
}
