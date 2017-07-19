using UnityEngine;
using System.Collections;
using Game.Logics.Classics;

namespace Game.Logics
{
  public class LogicActionInitializePlayerCommand : ILogicCommand
  {
    public void Execute(ILogicEngine engine, ILogicAction action)
    {
      var logic = ((LogicEngine)engine);
      var act = (LogicActionInitializePlayer) action;
      var players = logic.Modules.Get<LogicPlayerModule>();
      foreach (var playerId in act.Players)
      {
        players.GetPlayer(playerId);
      }
    }
  }
}