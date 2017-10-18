using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicMoveRunnerCommand : ClassicLogicCommand<MoveRunnerEvent>
  {
    protected override void Execute(MoveRunnerEvent evt, ClassicLogicEngine engine)
    {
      engine.Runner.transform.localPosition = engine.Level.CoordinateConverter.ToWorld(new PositionF((float)evt.x, engine.Level.Size.Y - 1 - (float)evt.y, 0));
    }
  }
}
