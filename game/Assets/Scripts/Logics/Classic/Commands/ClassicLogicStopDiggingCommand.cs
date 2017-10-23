using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicStopDiggingCommand : ClassicLogicCommand<StopDiggingEvent>
  {
    protected override void Execute(StopDiggingEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.StopDigging();
    }
  }
}
