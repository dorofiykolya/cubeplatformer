using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicFinishCommand : ClassicLogicCommand<FinishEvent>
  {
    protected override void Execute(FinishEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.Finish();
    }
  }
}
