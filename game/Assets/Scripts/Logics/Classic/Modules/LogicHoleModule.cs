using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Logics.Classics
{
  public class LogicHoleModule : ILogicModule
  {
    private LogicModules _logicModules;
    private Dictionary<int, LogicHole> _map;
    private List<LogicCellHole> _holeList;

    public LogicHoleModule(LogicModules logicModules)
    {
      _logicModules = logicModules;
      _map = new Dictionary<int, LogicHole>();
      _holeList = new List<LogicCellHole>();
    }

    public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      foreach (var logicHole in _map.Values)
      {
        if (logicHole.Tick())
        {
          _logicModules.Get<LogicPlayerModule>().digComplete(logicHole.PlayerId);
        }
      }
      ProcessFillHole();
    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void ProcessFillHole()
    {
      foreach (var hole in _holeList)
      {
        if (hole.IsFilled)
        {
          if (hole.Tick())
          {
            FillComplete(hole, hole.PlayerId);
          }
          break;
        }
      }
    }

    public void FillComplete(LogicCellHole data, int playerId)
    {
      //don't use "divide command", it will cause loss of accuracy while scale changed (ex: tileScale = 0.6...)
      //var x = this.x / tileWScale | 0; //this : scope default to the dispatcher
      //var y = this.y / tileHScale | 0;

      var map = _logicModules.Get<LogicGridModule>();

      _holeList.RemoveAll(h => h.Position == data.Position);

      var x = data.Position.X;
      var y = data.Position.Y;

      map[data.Position.X, data.Position.Y].Act = CellType.Block;

      _logicModules.Get<LogicViewModule>().DisplayBlock(data.Position);
      //map[data.Position.X, data.Position.Y].DisplayBlock();

      switch (map[x, y].Act)
      {
        case CellType.Player: // runner dead
                              //loadingTxt.text = "RUNNER DEAD"; 
                              //AI.State = CubeGameState.GAMEplayer_DEAD;

          _logicModules.Get<LogicViewModule>().HidePlayer(playerId);
          //player.Hide(); //hidden runner --> dead
          break;
        case CellType.Guard: //guard dead
          var guardModule = _logicModules.Get<LogicGuardModule>();
          int id = guardModule.getGuardId(x, y);
          if (map.curAiVersion >= 3 && guardModule[id].Action == LogicActorAction.InHole) guardModule.removeFromShake(id);
          if (guardModule[id].HasGold > 0)
          { //guard has gold and not fall into the hole
            _logicModules.Get<LogicCoinModule>().DecGold();
            guardModule[id].HasGold = 0;
          }
          guardModule.GuardReborn(x, y);
          //if(playMode == PLAY_CLASSIC || playMode == PLAY_AUTO || playMode == PLAY_DEMO) {	
          //    drawScore(SCORE_GUARD_DEAD);
          //} else {
          //for modern mode & edit mode
          //AI.drawGuard(1); //guard dead, add count
          //}
          break;
      }
      map[x, y].Act = CellType.Block;

      //if(DEBUG_TIME) loadingTxt.text = "FillHoleTime = " + (recordCount - fillHoleTimeStart); //for debug
    }

    public void FillHole(int x, int y, int playerId)
    {
      var hole = GetByPlayer(playerId);
      hole.Position.Set(x, y, 0);
      var current = new LogicCellHole
      {
        Position = new Position(x, y, 0),
        PlayerId = playerId
      };
      current.Start();
      _holeList.Add(current);

      /*var fillSprite = new createjs.Sprite(holeData, "fillHole");

      fillSprite.pos = { x:x, y:y }; //save position 11/18/2014
      fillSprite.setTransform(x * tileWScale, y * tileHScale, tileScale, tileScale);

      if(curAiVersion < 3) {
          fillSprite.on("animationend", fillComplete, null, false, {obj:fillSprite} );
          fillSprite.play();
      } else {
          fillSprite.curFrameIdx  =   0;
          fillSprite.curFrameTime =  -1;
          fillSprite.gotoAndStop(fillHoleFrame[0]);
      }
      mainStage.addChild(fillSprite); 
      fillHoleObj.push(fillSprite);
      */
      //fillHoleTimeStart = recordCount; //for debug
    }

    public LogicHole GetByPlayer(int playerId)
    {
      LogicHole result;
      if (!_map.TryGetValue(playerId, out result))
      {
        _map[playerId] = result = new LogicHole
        {
          PlayerId = playerId
        };
      }
      return result;
    }

    public LogicHole Get(int x, int y)
    {
      return _map.Values.FirstOrDefault(h => h.Position.X == x && h.Position.Y == y);
    }
  }
}