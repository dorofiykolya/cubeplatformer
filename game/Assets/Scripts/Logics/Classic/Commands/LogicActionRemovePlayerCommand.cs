using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{

  public class LogicActionRemovePlayerCommand : ILogicCommand
  {
    public void Execute(ILogicEngine engine, ILogicAction action)
    {
      var logic = ((LogicEngine)engine);
      var act = (LogicActionRemovePlayer)action;
      var players = logic.Modules.Get<LogicPlayerModule>();
      players.PlayerDead(act.PlayerId);
    }
  }
}