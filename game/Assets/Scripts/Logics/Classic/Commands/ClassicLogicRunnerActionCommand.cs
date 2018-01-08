using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicRunnerActionCommand : ClassicLogicCommand<RunnerActionEvent>
  {
    protected override void Execute(RunnerActionEvent evt, ClassicLogicEngine engine)
    {
      var runner = engine.ViewContext.Runner;
      CharacterTrigger currentTrigger;
      switch (evt.Action)
      {
        case Action.Stop:
          currentTrigger = runner.Trigger;
          switch (currentTrigger)
          {
            case CharacterTrigger.WalkLeft:
            case CharacterTrigger.Fall:
              runner.SetTrigger(CharacterTrigger.IdleLeft);
              break;
            case CharacterTrigger.WalkRight:
              runner.SetTrigger(CharacterTrigger.IdleRight);
              break;
            default:
              runner.Stop();
              break;
          }
          break;
        default:
          currentTrigger = runner.Trigger;
          switch (currentTrigger)
          {
            case CharacterTrigger.IdleLeft:
              runner.SetTrigger(CharacterTrigger.WalkLeft);
              break;
            case CharacterTrigger.IdleRight:
              runner.SetTrigger(CharacterTrigger.WalkRight);
              break;
            default:
              break;
          }
          break;
      }
    }
  }
}
