using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicAddGoldCommand : ClassicLogicCommand<AddGoldEvent>
  {
    protected override void Execute(AddGoldEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.AddGold(evt.X, evt.Y);
    }
  }
}
