using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicGuardShapeCommand : ClassicLogicCommand<GuardShapeEvent>
  {
    protected override void Execute(GuardShapeEvent evt, ClassicLogicEngine engine)
    {
      var guard = engine.ViewContext.GetGuard(evt.GuardId);
      switch (evt.Shape)
      {
        case Shape.RunLeft:
          guard.SetTrigger(CharacterTrigger.WalkLeft);
          break;
        case Shape.RunRight:
          guard.SetTrigger(CharacterTrigger.WalkRight);
          break;
        case Shape.BarLeft:
          guard.SetTrigger(CharacterTrigger.RopeLeft);
          break;
        case Shape.BarRight:
          guard.SetTrigger(CharacterTrigger.RopeRight);
          break;
        case Shape.DigHoleLeft:
          guard.SetTrigger(CharacterTrigger.FireLeft);
          break;
        case Shape.DigHoleRight:
          guard.SetTrigger(CharacterTrigger.FireRight);
          break;
        case Shape.RunUp:
          guard.SetTrigger(CharacterTrigger.LadderUp);
          break;
        case Shape.RunDown:
          guard.SetTrigger(CharacterTrigger.LadderDown);
          break;
        case Shape.FallLeft:
        case Shape.FallRight:
          guard.SetTrigger(CharacterTrigger.Fall);
          break;
      }
    }
  }
}
