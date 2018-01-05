using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicGuardActionCommand : ClassicLogicCommand<GuardActionEvent>
  {
    protected override void Execute(GuardActionEvent evt, ClassicLogicEngine engine)
    {
      var guard = engine.ViewContext.GetGuard(evt.GuardId);
      CharacterTrigger currentTrigger;
      switch (evt.Action)
      {
        case Action.Stop:
          currentTrigger = guard.Trigger;
          switch (currentTrigger)
          {
            case CharacterTrigger.WalkLeft:
              guard.SetTrigger(CharacterTrigger.IdleLeft);
              break;
            case CharacterTrigger.WalkRight:
              guard.SetTrigger(CharacterTrigger.IdleRight);
              break;
            default:
              guard.Stop();
              break;
          }
          break;
        default:
          currentTrigger = guard.Trigger;
          switch (currentTrigger)
          {
            case CharacterTrigger.IdleLeft:
              guard.SetTrigger(CharacterTrigger.WalkLeft);
              break;
            case CharacterTrigger.IdleRight:
              guard.SetTrigger(CharacterTrigger.WalkRight);
              break;
            default:
              break;
          }
          break;
      }
    }
  }
}
