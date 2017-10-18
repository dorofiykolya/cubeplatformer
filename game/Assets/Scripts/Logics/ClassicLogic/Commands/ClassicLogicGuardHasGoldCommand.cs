using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicGuardHasGoldCommand : ClassicLogicCommand<GuardHasGoldEvent>
  {
    protected override void Execute(GuardHasGoldEvent evt, ClassicLogicEngine engine)
    {
      engine.GetGuard(evt.Id).HaGold(evt.HasGold);
    }
  }
}
