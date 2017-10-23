using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicDigHoleProcessCommand : ClassicLogicCommand<DigHoleProcessEvent>
  {
    protected override void Execute(DigHoleProcessEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.DiggingProgress(evt.Ratio);
    }
  }
}
