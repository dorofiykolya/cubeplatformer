using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicRunnerDeadCommand : ClassicLogicCommand<RunnerDeadEvent>
  {
    protected override void Execute(RunnerDeadEvent evt, ClassicLogicEngine engine)
    {
      engine.Runner.gameObject.SetActive(false);
    }
  }
}
