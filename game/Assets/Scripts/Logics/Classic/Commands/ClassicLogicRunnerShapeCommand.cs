using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicRunnerShapeCommand : ClassicLogicCommand<RunnerShapeEvent>
  {
    protected override void Execute(RunnerShapeEvent evt, ClassicLogicEngine engine)
    {
      switch (evt.Shape)
      {
        case Shape.RunLeft:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.WalkLeft);
          break;
        case Shape.RunRight:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.WalkRight);
          break;
        case Shape.BarLeft:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.RopeLeft);
          break;
        case Shape.BarRight:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.RopeRight);
          break;
        case Shape.DigHoleLeft:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.FireLeft);
          break;
        case Shape.DigHoleRight:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.FireRight);
          break;
        case Shape.RunUp:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.LadderUp);
          break;
        case Shape.RunDown:
          engine.ViewContext.Runner.SetTrigger(CharacterTrigger.LadderDown);
          break;
      }
    }
  }
}
