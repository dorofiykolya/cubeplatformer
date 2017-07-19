using UnityEngine;
using System.Collections;

namespace Game.Logics.Classics
{
  public class LogicActionAddPlayerCommand : ILogicCommand
  {
    public void Execute(ILogicEngine engine, ILogicAction action)
    {
      var logic = ((LogicEngine)engine);
      var act = (LogicActionAddPlayer)action;
      var players = logic.Modules.Get<LogicPlayerModule>();
      players.GetPlayer(act.PlayerId);
    }
  }
}