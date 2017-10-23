using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicDiggingCompleteCommand : ClassicLogicCommand<DiggingCompleteEvent>
  {
    protected override void Execute(DiggingCompleteEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.DiggingComplete();
    }
  }
}
 