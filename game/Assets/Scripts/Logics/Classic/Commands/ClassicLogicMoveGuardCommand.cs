﻿using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicMoveGuardCommand : ClassicLogicCommand<MoveGuardEvent>
  {
    protected override void Execute(MoveGuardEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.MoveGuard(evt.Id, evt.X, evt.Y);
    }
  }
}
