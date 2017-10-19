using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicStartDiggingCommand : ClassicLogicCommand<StartDiggingEvent>
  {
    protected override void Execute(StartDiggingEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.StartDigging();
    }
  }
}
