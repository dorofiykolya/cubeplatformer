using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicStartDiggingCommand : ClassicLogicCommand<StartDiggingEvent>
  {
    protected override void Execute(StartDiggingEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.StartDigging();
    }
  }
}
