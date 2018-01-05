using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicRunnerShapeCommand : ClassicLogicCommand<RunnerShapeEvent>
  {
    protected override void Execute(RunnerShapeEvent evt, ClassicLogicEngine engine)
    {
      var runner = engine.ViewContext.Runner;
      switch (evt.Shape)
      {
        case Shape.RunLeft:
          runner.SetTrigger(CharacterTrigger.WalkLeft);
          break;
        case Shape.RunRight:
          runner.SetTrigger(CharacterTrigger.WalkRight);
          break;
        case Shape.BarLeft:
          runner.SetTrigger(CharacterTrigger.RopeLeft);
          break;
        case Shape.BarRight:
          runner.SetTrigger(CharacterTrigger.RopeRight);
          break;
        case Shape.DigHoleLeft:
          runner.SetTrigger(CharacterTrigger.FireLeft);
          break;
        case Shape.DigHoleRight:
          runner.SetTrigger(CharacterTrigger.FireRight);
          break;
        case Shape.RunUp:
          runner.SetTrigger(CharacterTrigger.LadderUp);
          break;
        case Shape.RunDown:
          runner.SetTrigger(CharacterTrigger.LadderDown);
          break;
      }
    }
  }
}
