using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicMoveGuardCommand : ClassicLogicCommand<MoveGuardEvent>
  {
    protected override void Execute(MoveGuardEvent evt, ClassicLogicEngine engine)
    {
      var guard = engine.GetGuard(evt.Id);
      guard.transform.localPosition = engine.Level.CoordinateConverter.ToWorld(new PositionF((float)evt.X, engine.Level.Size.Y - 1 - (float)evt.Y, 0));
    }
  }
}
