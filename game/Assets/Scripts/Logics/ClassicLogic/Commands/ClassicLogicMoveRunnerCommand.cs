﻿using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicMoveRunnerCommand : ClassicLogicCommand<MoveRunnerEvent>
  {
    protected override void Execute(MoveRunnerEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.MoveRunner(evt.x, evt.y);
    }
  }
}
