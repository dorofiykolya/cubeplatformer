﻿using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicGuardHasGoldCommand : ClassicLogicCommand<GuardHasGoldEvent>
  {
    protected override void Execute(GuardHasGoldEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.GetGuard(evt.Id).HaGold(evt.HasGold);
    }
  }
}
