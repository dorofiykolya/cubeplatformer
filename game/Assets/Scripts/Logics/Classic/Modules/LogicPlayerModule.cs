using UnityEngine;
using System.Collections;
using Game.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Random = UnityEngine.Random;

namespace Game.Logics.Classics
{
  public class LogicPlayerModule : ILogicModule
  {
    public const int STATE_OK_TO_MOVE = 1;
    public const int STATE_FALLING = 2;

    private int _nextIndex;
    private LogicModules _logicModules;
    private List<Position> _spawnPoints;
    private List<LogicPlayer> _players;

    public LogicPlayerModule(LogicModules logicModules)
    {
      _spawnPoints = new List<Position>();
      _logicModules = logicModules;
      _players = new List<LogicPlayer>();
    }

    public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      foreach (var player in _players)
      {
        if (IsDigging(player.Id))
        {
          Move(player.Id);
        }
      }
    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void Move(int playerId)
    {
      var stayCurrPos = true;
      var map = _logicModules.Get<LogicGridModule>();
      var player = GetPlayer(playerId);
      var x = player.Position.X;
      var xOffset = player.xOffset;
      var y = player.Position.Y;
      var yOffset = player.yOffset;

      var curToken = map[x, y].Base;
      int curState;
      CellType nextToken;
      if (curToken == CellType.Ladder || (curToken == CellType.Rope && Math.Abs(yOffset) <= float.Epsilon))
      { //ladder & bar
        curState = STATE_OK_TO_MOVE; //ok to move (on ladder or bar)
      }
      else if (yOffset < 0)
      {  //no ladder && yOffset < 0 ==> falling 
        curState = STATE_FALLING;
      }
      else if (y < map.MaxTileY)
      { //no laddr && y < maxTileY && yOffset >= 0

        nextToken = map[x, y + 1].Act;
        if (nextToken == CellType.Empty)
        {
          curState = STATE_FALLING;
        }
        else if (nextToken == CellType.Block ||
                nextToken == CellType.Ladder ||
                nextToken == CellType.Solid)
        {
          curState = STATE_OK_TO_MOVE;
        }
        else if (nextToken == CellType.Guard)
        {
          curState = STATE_OK_TO_MOVE;
        }
        else
        {
          curState = STATE_FALLING;
        }

      }
      else
      { // no laddr && y == maxTileY 
        curState = STATE_OK_TO_MOVE;
      }

      if (curState == STATE_FALLING)
      {
        stayCurrPos = (y >= map.MaxTileY ||
            (nextToken = map[x, y + 1].Act) == CellType.Block ||
             nextToken == CellType.Solid || nextToken == CellType.Guard);

        runnerMoveStep(LogicActorAction.Fall, stayCurrPos, playerId);
        return;
      }

      /****** Check Key Action ******/

      var moveStep = LogicActorAction.Stop;
      stayCurrPos = true;
      LogicActorAction keyAction = _logicModules.Get<LogicUserInputModule>().KeyAction;
      switch (keyAction)
      {
        case LogicActorAction.Up:
          stayCurrPos = (y <= 0 || (nextToken = map[x, y - 1].Act) == CellType.Block ||
                 nextToken == CellType.Solid || nextToken == CellType.Trap);

          if (y > 0 && map[x, y].Base != CellType.Ladder && yOffset < map.H4 && yOffset > 0 && map[x, y + 1].Base == CellType.Ladder)
          {
            stayCurrPos = true;
            moveStep = LogicActorAction.Up;
          }
          else
              if (!(map[x, y].Base != CellType.Ladder &&
                  (yOffset <= 0 || map[x, y + 1].Base != CellType.Ladder) ||
                  (yOffset <= 0 && stayCurrPos))
              )
          {
            moveStep = LogicActorAction.Up;
          }

          break;
        case LogicActorAction.Down:
          stayCurrPos = (y >= map.MaxTileY ||
              (nextToken = map[x, y + 1].Act) == CellType.Block ||
               nextToken == CellType.Solid);

          if (!(yOffset >= 0 && stayCurrPos))
            moveStep = LogicActorAction.Down;
          break;
        case LogicActorAction.Left:
          stayCurrPos = (x <= 0 ||
              (nextToken = map[x - 1, y].Act) == CellType.Block ||
               nextToken == CellType.Solid || nextToken == CellType.Trap);

          if (!(xOffset <= 0 && stayCurrPos))
            moveStep = LogicActorAction.Left;
          break;
        case LogicActorAction.Right:
          stayCurrPos = (x >= map.MaxTileX ||
              (nextToken = map[x + 1, y].Act) == CellType.Block ||
               nextToken == CellType.Solid || nextToken == CellType.Trap);

          if (!(xOffset >= 0 && stayCurrPos))
            moveStep = LogicActorAction.Right;
          break;
        case LogicActorAction.DigLeft:
        case LogicActorAction.DigRight:
          if (ok2Dig(keyAction, playerId))
          {
            runnerMoveStep(keyAction, stayCurrPos, playerId);
            digHole(keyAction, playerId);
          }
          else
          {
            runnerMoveStep(LogicActorAction.Stop, stayCurrPos, playerId);
          }
          keyAction = LogicActorAction.Stop;
          _logicModules.Get<LogicUserInputModule>().KeyAction = keyAction;
          return;
      }
      runnerMoveStep(moveStep, stayCurrPos, playerId);
    }

    public bool ok2Dig(LogicActorAction nextMove, int playerId)
    {
      var player = GetPlayer(playerId);
      var x = player.Position.X;
      var y = player.Position.Y;
      var map = _logicModules.Get<LogicGridModule>();
      var level = map;
      var rc = false;

      switch (nextMove)
      {
        case LogicActorAction.DigLeft:
          if (y < level.MaxTileY && x > 0 && map[x - 1, y + 1].Act == CellType.Block &&
              map[x - 1, y].Act == CellType.Empty && map[x - 1, y].Base != CellType.Gold)
            rc = true;
          break;
        case LogicActorAction.DigRight:
          if (y < level.MaxTileY && x < level.MaxTileX && map[x + 1, y + 1].Act == CellType.Block &&
              map[x + 1, y].Act == CellType.Empty && map[x + 1, y].Base != CellType.Gold)
            rc = true;
          break;
      }

      return rc;
    }

    public void digHole(LogicActorAction action, int playerId)
    {
      var player = GetPlayer(playerId);
      var map = _logicModules.Get<LogicGridModule>();

      int x;
      int y;
      string holeShape;

      if (action == LogicActorAction.DigLeft)
      {
        x = player.Position.X - 1;
        y = player.Position.Y;

        player.shape = LogicActorAction.DigLeft;
        holeShape = "digHoleLeft";
      }
      else
      { //DIG RIGHT

        x = player.Position.X + 1;
        y = player.Position.Y;

        player.shape = LogicActorAction.DigRight;
        holeShape = "digHoleRight";
      }

      _logicModules.Get<LogicSoundModule>().PlayDig();
      //map[x, y + 1].HideBlock(); //hide block (replace with digging image)

      _logicModules.Get<LogicViewModule>().HideBlock(x, y + 1);

      _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, player.shape);


      var holeObj = _logicModules.Get<LogicHoleModule>().GetByPlayer(playerId);
      holeObj.Action = LogicActorAction.Digging;
      holeObj.Position.Set(x, y, 0);
      //holeObj.sprite.setTransform(x, y);
      holeObj.StartDigging(_logicModules.Get<LogicGridModule>()[x, y + 1]);


      //digTimeStart = recordCount; //for debug
      _logicModules.Get<LogicViewModule>().HoleObjPlay(playerId, player.shape);
      //holeObj.sprite.gotoAndPlay(player.shape);
      //holeObj.sprite.onComplete(digComplete);

      _logicModules.Get<LogicViewModule>().HoleObjAddToScene(holeObj);
    }

    public void digComplete(int playerId)
    {
      var holeObj = _logicModules.Get<LogicHoleModule>().GetByPlayer(playerId);
      var map = _logicModules.Get<LogicGridModule>();
      var x = holeObj.Position.X;
      var y = holeObj.Position.Y + 1;

      map[x, y].Act = CellType.Empty;
      //            holeObj.sprite.removeAllEventListeners("animationend");
      holeObj.Action = LogicActorAction.Stop; //no digging

      _logicModules.Get<LogicViewModule>().HoleObjRemoveFromScene(holeObj);

      //mainStage.removeChild(holeObj.sprite);

      //if (DEBUG_TIME) loadingTxt.text = "DigTime = " + (recordCount - digTimeStart);
      _logicModules.Get<LogicHoleModule>().FillHole(x, y, playerId);
    }



    public void runnerMoveStep(LogicActorAction action, bool stayCurrPos, int playerId)
    {
      var player = GetPlayer(playerId);
      var x = player.Position.X;
      var xOffset = player.xOffset;
      var y = player.Position.Y;
      var yOffset = player.yOffset;
      var map = _logicModules.Get<LogicGridModule>();
      var guardModule = _logicModules.Get<LogicGuardModule>();

      var curShape = player.shape;
      var newShape = curShape;

      var centerX = LogicActorAction.Stop;
      var centerY = centerX;

      switch (action)
      {
        case LogicActorAction.DigLeft:
        case LogicActorAction.DigRight:
          xOffset = 0;
          yOffset = 0;
          break;
        case LogicActorAction.Up:
        case LogicActorAction.Down:
        case LogicActorAction.Fall:
          if (xOffset > 0) centerX = LogicActorAction.Left;
          else if (xOffset < 0) centerX = LogicActorAction.Right;
          break;
        case LogicActorAction.Left:
        case LogicActorAction.Right:
          if (yOffset > 0) centerY = LogicActorAction.Up;
          else if (yOffset < 0) centerY = LogicActorAction.Down;
          break;
      }

      var curToken = map[x, y].Base;

      if (action == LogicActorAction.Up)
      {
        yOffset -= player.yMove;

        if (stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
        else if (yOffset < -map.H2)
        { //move to y-1 position 
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
          y--;
          yOffset = map.TileH + yOffset;
          if (map[x, y].Act == CellType.Guard && _logicModules.Get<LogicGuardModule>().GuardAlive(x, y)) PlayerDead(playerId); //collision
        }
        //newShape = "runUpDn";
      }

      if (centerY == LogicActorAction.Up)
      {
        yOffset -= player.yMove;
        if (yOffset < 0) yOffset = 0; //move to center Y	
      }

      if (action == LogicActorAction.Down || action == LogicActorAction.Fall)
      {
        var holdOnBar = 0;
        if (curToken == CellType.Rope)
        {
          if (yOffset < 0) holdOnBar = 1;
          else if (action == LogicActorAction.Down && y < map.MaxTileY && map[x, y + 1].Act != CellType.Ladder)
          {
            action = LogicActorAction.Fall; //shape fixed: 2014/03/27
          }
        }

        yOffset += player.yMove;

        if (holdOnBar == 1 && yOffset >= 0)
        {
          yOffset = 0; //fall and hold on bar
          action = LogicActorAction.FallBar;
        }
        if (stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
        else if (yOffset > map.H2)
        { //move to y+1 position
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
          y++;
          yOffset = yOffset - map.TileH;
          if (map[x, y].Act == CellType.Guard && guardModule.GuardAlive(x, y)) PlayerDead(playerId); //collision
        }

        if (action == LogicActorAction.Down)
        {
          //newShape = "runUpDn";
        }
        else
        { //ACT_FALL or ACT_FALL_BAR

          if (y < map.MaxTileY && map[x, y + 1].Act == CellType.Guard)
          { //over guard
            //don't collision
            var id = guardModule.getGuardId(x, y + 1);
            if (yOffset > guardModule[id].YOffset) yOffset = guardModule[id].YOffset;
          }

          if (action == LogicActorAction.FallBar)
          {
            //if (player.lastLeftRight == LogicActorAction.Left) newShape = "barLeft";
            //else newShape = "barRight";
          }
          else
          {
            //if (player.lastLeftRight == LogicActorAction.Left) newShape = "fallLeft";
            //else newShape = "fallRight";
          }
        }
      }

      if (centerY == LogicActorAction.Down)
      {
        yOffset += player.yMove;
        if (yOffset > 0) yOffset = 0; //move to center Y
      }

      if (action == LogicActorAction.Left)
      {
        xOffset -= player.xMove;

        if (stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
        else if (xOffset < -map.W2)
        { //move to x-1 position 
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
          x--;
          xOffset = map.TileW + xOffset;
          if (map[x, y].Act == CellType.Guard && guardModule.GuardAlive(x, y)) PlayerDead(playerId); //collision
        }
        //if (curToken == CellType.RopeBar) newShape = "barLeft";
        //else newShape = "runLeft";
      }

      if (centerX == LogicActorAction.Left)
      {
        xOffset -= player.xMove;
        if (xOffset < 0) xOffset = 0; //move to center X
      }

      if (action == LogicActorAction.Right)
      {
        xOffset += player.xMove;

        if (stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
        else if (xOffset > map.W2)
        { //move to x+1 position 
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
          x++;
          xOffset = xOffset - map.TileW;
          if (map[x, y].Act == CellType.Guard && guardModule.GuardAlive(x, y)) PlayerDead(playerId); //collision
        }
        //if (curToken == CellType.RopeBar) newShape = "barRight";
        //else newShape = "runRight";
      }

      if (centerX == LogicActorAction.Right)
      {
        xOffset += player.xMove;
        if (xOffset > 0) xOffset = 0; //move to center X
      }

      if (action == LogicActorAction.Stop)
      {
        if (player.Action == LogicActorAction.Fall)
        {
          _logicModules.Get<LogicSoundModule>().StopFall();
          _logicModules.Get<LogicSoundModule>().PlayDown();
        }
        if (player.Action != LogicActorAction.Stop)
        {
          _logicModules.Get<LogicViewModule>().PlayerStop(playerId);

          player.Action = LogicActorAction.Stop;
        }
      }
      else
      {
        _logicModules.Get<LogicViewModule>().PlayerPosition(playerId, (x + xOffset), (y + yOffset));

        player.Position.Set(x, y, 0);
        player.SetOffset(xOffset, yOffset);
        if (curShape != newShape)
        {
          _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, newShape);

          player.shape = newShape;
        }
        if (action != player.Action)
        {
          if (player.Action == LogicActorAction.Fall)
          {
            _logicModules.Get<LogicSoundModule>().StopFall();
            _logicModules.Get<LogicSoundModule>().PlayDown();
          }
          else if (action == LogicActorAction.Fall)
          {
            _logicModules.Get<LogicSoundModule>().PlayFall();
          }
          _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, player.shape);
        }
        if (action == LogicActorAction.Left || action == LogicActorAction.Right) player.LastAction = action;
        player.Action = action;
      }
      map[x, y].Act = CellType.Player;

      // Check runner to get gold (MAX MOVE MUST < H4 & W4) 
      if (map[x, y].Base == CellType.Gold &&
          ((xOffset == 0f && yOffset >= 0 && yOffset < map.H4) ||
           (yOffset == 0f && xOffset >= 0 && xOffset < map.W4) ||
           (y < map.MaxTileY && map[x, y + 1].Base == CellType.Ladder && yOffset < map.H4) // gold above laddr
          )
        )
      {
        _logicModules.Get<LogicCoinModule>().RemoveGold(x, y);
        _logicModules.Get<LogicSoundModule>().PlayGetGold();
        _logicModules.Get<LogicCoinModule>().DecGold();
        //debug("gold = " + goldCount);
        //if (playMode == PLAY_CLASSIC || playMode == PLAY_AUTO || playMode == PLAY_DEMO)
        //{
        //    drawScore(SCORE_GET_GOLD);
        //}
        //else
        //{
        //for modern mode , edit mode
        //AI.drawGold(1); //get gold 
        //}
      }
      if (_logicModules.Get<LogicCoinModule>().goldCount == 0 && !_logicModules.Get<LogicCoinModule>().goldComplete) showHideLaddr();

      //check collision with guard !
      checkCollision(x, y, playerId);
    }

    public bool showHideLaddr()
    {
      var map = _logicModules.Get<LogicGridModule>();
      var haveHLadder = false;
      for (var y = 0; y < map.MaxTileY; y++)
      {
        for (var x = 0; x < map.MaxTileX; x++)
        {
          if (map[x, y].Base == CellType.HLadr)
          {
            haveHLadder = true;
            map[x, y].Base = CellType.Ladder;
            map[x, y].Act = CellType.Ladder;

            _logicModules.Get<LogicViewModule>().DisplayBlock(new Position(x, y, 0));
            //            map[x, y].DisplayBlock(); //display laddr
          }
        }
      }
      _logicModules.Get<LogicCoinModule>().goldComplete = true;
      return haveHLadder;
    }

    public void PlayerDead(int playerId)
    {

    }

    public void checkCollision(int runnerX, int runnerY, int playerId)
    {
      var player = GetPlayer(playerId);
      var x = -1;
      var y = -1;
      var map = _logicModules.Get<LogicGridModule>();
      var guardModule = _logicModules.Get<LogicGuardModule>();
      //var dbg = "NO";

      if (runnerY > 0 && map[runnerX, runnerY - 1].Act == CellType.Guard)
      {
        x = runnerX;
        y = runnerY - 1;
        //dbg = "UP";	
      }
      else if (runnerY < map.MaxTileY && map[runnerX, runnerY + 1].Act == CellType.Guard)
      {
        x = runnerX;
        y = runnerY + 1;
        //dbg = "DN";	
      }
      else if (runnerX > 0 && map[runnerX - 1, runnerY].Act == CellType.Guard)
      {
        x = runnerX - 1;
        y = runnerY;
        //dbg = "LF";	
      }
      else if (runnerX < map.MaxTileX && map[runnerX + 1, runnerY].Act == CellType.Guard)
      {
        x = runnerX + 1;
        y = runnerY;
        //dbg = "RT";	
      }

      //if( dbg != "NO") debug(dbg);
      if (x >= 0)
      {
        var i = 0;
        for (; i < guardModule.guardCount; i++)
        {
          if (guardModule[i].Position.X == x && guardModule[i].Position.Y == y) break;
        }
        Assert2.IsTrue((i < guardModule.guardCount), "checkCollision design error !");
        if (guardModule[i].Action != LogicActorAction.Reborn)
        { //only guard alive need check collection
          //var dw = Math.abs(runner.sprite.x - guard[i].sprite.x);
          //var dh = Math.abs(runner.sprite.y - guard[i].sprite.y);

          //change detect method ==> don't depend on scale 
          var runnerPosX = player.Position.X * map.TileW + player.xOffset;
          var runnerPosY = player.Position.Y * map.TileH + player.yOffset;
          var guardPosX = guardModule[i].Position.X * map.TileW + guardModule[i].XOffset;
          var guardPosY = guardModule[i].Position.Y * map.TileH + guardModule[i].YOffset;

          var dw = Math.Abs(runnerPosX - guardPosX);
          var dh = Math.Abs(runnerPosY - guardPosY);

          if (dw <= map.W4 * 3 && dh <= map.H4 * 3)
          {
            PlayerDead(playerId); //07/04/2014
                                  //debug("runner dead!");
          }
        }
      }
    }

    public bool IsDigging(int playerId)
    {
      var player = GetPlayer(playerId);
      var map = _logicModules.Get<LogicGridModule>();
      var guardModule = _logicModules.Get<LogicGuardModule>();
      var holeObj = _logicModules.Get<LogicHoleModule>().GetByPlayer(playerId);
      if (holeObj.Action == LogicActorAction.Digging)
      {
        var x = holeObj.Position.X;
        var y = holeObj.Position.Y;
        if (map[x, y].Act == CellType.Guard)
        {
          var guard = guardModule.Get(x, y);
          if (holeObj.IsDigging && guard.YOffset > map.H4)
          {
            StopDigging(x, y, playerId);
          }
        }
        else
        {
          switch (player.shape)
          {
            case LogicActorAction.DigLeft:
              //if (holeObj.sprite.currentAnimationFrame > 2)
              {
                _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, LogicActorAction.Left);

                player.shape = LogicActorAction.Left;
                player.Action = LogicActorAction.Stop;
              }
              break;
            case LogicActorAction.DigRight:
              //if (holeObj.sprite.currentAnimationFrame > 2)
              {
                _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, LogicActorAction.Right);

                player.shape = LogicActorAction.Right;
                player.Action = LogicActorAction.Stop;
              }
              break;
          }
          return true;
        }
      }
      return false;
    }

    private void StopDigging(int x, int y, int playerId)
    {
      var player = GetPlayer(playerId);
      var map = _logicModules.Get<LogicGridModule>();
      var holeObj = _logicModules.Get<LogicHoleModule>().GetByPlayer(playerId);
      // remove hole
      holeObj.Action = LogicActorAction.Stop;

      _logicModules.Get<LogicViewModule>().HoleObjRemoveFromScene(holeObj);

      //fill hole
      y++;
      map[x, y].Act = map[x, y].Base; // BLOCK
      Assert2.IsTrue(map[x, y].Base == CellType.Block, "fill hole != BLOCK");

      _logicModules.Get<LogicViewModule>().DisplayBlock(new Position(x, y, 0));

      //change runner shape
      switch (player.shape)
      {
        case LogicActorAction.DigLeft:
          player.shape = LogicActorAction.Left;
          player.Action = LogicActorAction.Stop;
          _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, LogicActorAction.Left);

          break;
        case LogicActorAction.DigRight:
          player.shape = LogicActorAction.Right;
          player.Action = LogicActorAction.Stop;
          _logicModules.Get<LogicViewModule>().PlayerPlay(playerId, LogicActorAction.Right);
          break;
      }

      _logicModules.Get<LogicSoundModule>().StopDig();
    }

    public void AddPlayer(CellComponent cell)
    {
      _spawnPoints.Add(cell.Position);
    }

    public LogicPlayer GetPlayer(int id)
    {
      var player = _players.FirstOrDefault(p => p.Id == id);
      if (player == null)
      {
        var position = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        player = new LogicPlayer
        {
          Position = position,
          Id = id
        };
        _players.Add(player);
        _logicModules.Get<LogicViewModule>().PlayerAdd(player);
      }

      return player;
    }

    public int GetNextPlayerId()
    {
      if (_players.Count == 0) return -1;
      var result = _players[_nextIndex].Id;
      _nextIndex++;
      if (_nextIndex >= _players.Count) _nextIndex = 0;
      return result;
    }
  }
}