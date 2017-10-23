using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicStartFillHoleCommand : ClassicLogicCommand<StartFillHoleEvent>
  {
    protected override void Execute(StartFillHoleEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.StartFillHole(evt.X, evt.Y);
    }
  }
}
