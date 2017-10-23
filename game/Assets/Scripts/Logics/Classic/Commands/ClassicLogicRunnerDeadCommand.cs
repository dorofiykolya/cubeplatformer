using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicRunnerDeadCommand : ClassicLogicCommand<RunnerDeadEvent>
  {
    protected override void Execute(RunnerDeadEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.RunnerDead();
    }
  }
}
