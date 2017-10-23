using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicEndFillHoleCommand : ClassicLogicCommand<EndFillHoleEvent>
  {
    protected override void Execute(EndFillHoleEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.EndFillHole(evt.X, evt.Y);
    }
  }
}
