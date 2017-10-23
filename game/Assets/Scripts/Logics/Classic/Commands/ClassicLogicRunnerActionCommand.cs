using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;
using Game.Logics.Classic;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicRunnerActionCommand : ClassicLogicCommand<RunnerActionEvent>
  {
    protected override void Execute(RunnerActionEvent evt, ClassicLogicEngine engine)
    {
      CharacterTrigger currentTrigger;
      switch (evt.Action)
      {
        case Action.Stop:
          currentTrigger = engine.ViewContext.Runner.Trigger;
          switch (currentTrigger)
          {
            case CharacterTrigger.WalkLeft:
              engine.ViewContext.Runner.SetTrigger(CharacterTrigger.IdleLeft);
              break;
            case CharacterTrigger.WalkRight:
              engine.ViewContext.Runner.SetTrigger(CharacterTrigger.IdleRight);
              break;
            default:
              engine.ViewContext.Runner.Stop();
              break;
          }
          break;
        default:
          currentTrigger = engine.ViewContext.Runner.Trigger;
          switch (currentTrigger)
          {
            case CharacterTrigger.IdleLeft:
              engine.ViewContext.Runner.SetTrigger(CharacterTrigger.WalkLeft);
              break;
            case CharacterTrigger.IdleRight:
              engine.ViewContext.Runner.SetTrigger(CharacterTrigger.WalkRight);
              break;
            default:
              break;
          }
          break;
      }
    }
  }
}
