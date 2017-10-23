using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicShowTrapCommand : ClassicLogicCommand<ShowTrapEvent>
  {
    protected override void Execute(ShowTrapEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.ShowTrap(evt.X, evt.Y);
    }
  }
}
