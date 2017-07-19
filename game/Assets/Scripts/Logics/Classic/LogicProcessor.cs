using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Logics.Classics
{
  public class LogicProcessor
  {
    private Dictionary<Type, ILogicCommand> _map = new Dictionary<Type, ILogicCommand>();

    public LogicProcessor()
    {
      _map[typeof(LogicActionInitializePlayer)] = new LogicActionInitializePlayerCommand();
      _map[typeof(LogicActionAddPlayer)] = new LogicActionAddPlayerCommand();
      _map[typeof(LogicActionRemovePlayer)] = new LogicActionRemovePlayerCommand();
    }

    public void Execute(ILogicAction currentAction, LogicEngine logicEngine)
    {
      ILogicCommand command;
      if (_map.TryGetValue(currentAction.GetType(), out command))
      {
        command.Execute(logicEngine, currentAction);
      }
    }
  }
}