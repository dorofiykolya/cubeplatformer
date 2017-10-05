using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public class GuardHasGoldCommand : PlayCommand<GuardHasGoldEvent>
  {
    protected override void Execute(GuardHasGoldEvent value, TileContainer tileContainer)
    {
      tileContainer.GuardHasGold(value.Id, value.HasGold);
    }
  }
}
