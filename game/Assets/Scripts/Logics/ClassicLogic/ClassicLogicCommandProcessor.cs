using System;
using System.Collections.Generic;
using ClassicLogic.Outputs;
using Game.Logics.ClassicLogic.Commands;
using UnityEngine;

namespace Game.Logics.ClassicLogic
{
  public class ClassicLogicCommandProcessor
  {
    private readonly Dictionary<Type, ClassicLogicCommand> _map = new Dictionary<Type, ClassicLogicCommand>();

    public ClassicLogicCommandProcessor()
    {
      AddCommand<InitializeEvent, ClassicLogicInitializeCommand>();
      AddCommand<MoveRunnerEvent, ClassicLogicMoveRunnerCommand>();
      AddCommand<MoveGuardEvent, ClassicLogicMoveGuardCommand>();
      AddCommand<StartFillHoleEvent, ClassicLogicStartFillHoleCommand>();
      AddCommand<EndFillHoleEvent, ClassicLogicEndFillHoleCommand>();
      AddCommand<ShowTrapEvent, ClassicLogicShowTrapCommand>();
      AddCommand<StartDiggingEvent, ClassicLogicStartDiggingCommand>();
      AddCommand<StopDiggingEvent, ClassicLogicStopDiggingCommand>();
      AddCommand<ShowHideLadderEvent, ClassicLogicShowHideLadderCommand>();
      AddCommand<RunnerDeadEvent, ClassicLogicRunnerDeadCommand>();
      AddCommand<DigHoleProcessEvent, ClassicLogicDigHoleProcessCommand>();
      AddCommand<DiggingCompleteEvent, ClassicLogicDiggingCompleteCommand>();
      AddCommand<GuardHasGoldEvent, ClassicLogicGuardHasGoldCommand>();
      AddCommand<AddGoldEvent, ClassicLogicAddGoldCommand>();
      AddCommand<RemoveGoldEvent, ClassicLogicRemoveGoldCommand>();
    }

    public void Execute(OutputEvent evt, ClassicLogicEngine engine)
    {
      ClassicLogicCommand command;
      if (_map.TryGetValue(evt.GetType(), out command))
      {
        command.Execute(evt, engine);
      }
      else
      {
        Debug.Log("not implemented event: " + evt.GetType());
      }
    }

    private void AddCommand<TEvent, TCommand>() where TCommand : ClassicLogicCommand<TEvent>, new() where TEvent : OutputEvent
    {
      var command = new TCommand();
      _map[typeof(TEvent)] = command;
    }
  }
}
