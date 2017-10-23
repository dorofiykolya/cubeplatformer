using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicRemoveGoldCommand : ClassicLogicCommand<RemoveGoldEvent>
  {
    protected override void Execute(RemoveGoldEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.RemoveGold(evt.X, evt.Y);
    }
  }
}
