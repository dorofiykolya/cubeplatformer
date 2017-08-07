using UnityEngine;
using System.Collections;
using Game.Views.Components;
using System;
using System.Collections.Generic;
using Utils;
using Random = UnityEngine.Random;

namespace Game.Logics.Classics
{
  public class LogicGuardModule : ILogicModule
  {
    private LogicModules _logicModules;
    private List<Position> _spawnPoints;

    public int guardCount;
    public int moveOffset;
    public int moveId;
    public int numOfMoveItems;
    public int bornRndX;
    public List<int> rebornGuardList;
    public List<LogicGuard> _guards;
    public int[][] movePolicy = new[]{
                   new []{0, 0, 0, 0, 0, 0}, //* move_map is used to find *//
                   new []{0, 1, 1, 0, 1, 1}, //* wheather to move a enm   *//
                   new []{1, 1, 1, 1, 1, 1}, //* by indexing into it with *//
                   new []{1, 2, 1, 1, 2, 1}, //* enm_byte + num_enm +     *//
                   new []{1, 2, 2, 1, 2, 2}, //* set_num to get a byte.   *//
                   new []{2, 2, 2, 2, 2, 2}, //* then that byte is checked*//
                   new []{2, 2, 3, 2, 2, 3}, //* for !=0 and then decrmnt *//
                   new []{2, 3, 3, 2, 3, 3}, //* for next test until = 0  *// 
                   new []{3, 3, 3, 3, 3, 3},
                   new []{3, 3, 4, 3, 3, 4},
                   new []{3, 4, 4, 3, 4, 4},
                   new []{4, 4, 4, 4, 4, 4}
         };

    public LogicGuardModule(LogicModules logicModules)
    {
      _logicModules = logicModules;
      _spawnPoints = new List<Position>();

      numOfMoveItems = movePolicy[0].Length;
      rebornGuardList = new List<int>();
      _guards = new List<LogicGuard>();

      moveOffset = moveId = 0;
    }

    public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      Move();
    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void initRnd()
    {
      //bornRndX = new rangeRandom(0, maxTileX, curLevel); //set fixed seed for demo mode
      bornRndX = Random.Range(0, _logicModules.Get<LogicGridModule>().MaxTileX); //random range 0 .. maxTileX
    }

    public void Move()
    {
      int moves;
      int x, y, yOffset;

      if (guardCount == 0) return; //no guard

      if (++moveOffset >= numOfMoveItems) moveOffset = 0;
      moves = movePolicy[guardCount][moveOffset];  // get next moves 

      var players = _logicModules.Get<LogicPlayerModule>();

      while (moves-- > 0)
      {                       // slows guard relative to runner
        if (++moveId >= guardCount) moveId = 0;
        var curGuard = this[moveId];

        if (curGuard.Action == LogicActorAction.InHole || curGuard.Action == LogicActorAction.Reborn)
        {
          continue;
        }
        var playerId = players.GetNextPlayerId();
        if (playerId != -1)
        {
          guardMoveStep(moveId, bestMove(moveId, playerId), playerId);
        }
      }
    }

    public LogicGuard Get(int x, int y)
    {
      return this[getGuardId(x, y)];
    }

    public int getGuardId(int x, int y)
    {
      int id = 0;

      for (id = 0; id < guardCount; id++)
      {
        if (this[id].Position.X == x && this[id].Position.Y == y) break;
      }
      Assert2.IsTrue(id < guardCount, "Error: can not get guard position!");

      return id;
    }

    public int Add(LogicGuard guard)
    {
      _guards.Add(guard);
      guard.Id = guardCount;
      return guardCount++;
    }

    public void removeFromShake(int id)
    {

    }

    public void GuardReborn(int x, int y)
    {
      var map = _logicModules.Get<LogicGridModule>();
      var level = _logicModules.Get<LogicGridModule>();
      //get guard id  by current in hole position
      var id = getGuardId(x, y);

      var bornY = 1; //start on line 2
      var bornX = Random.Range(0, level.MaxTileX);//bornRndX.get();
      var rndStart = bornX;


      while (map[bornX, bornY].Act != CellType.Empty || map[bornX, bornY].Base == CellType.Gold || map[bornX, bornY].Base == CellType.Block)
      {
        //BUG FIXED for level 115 (can not reborn at bornX=27)
        //don't born at gold position & diged position, 2/24/2015
        if ((bornX = Random.Range(0, level.MaxTileX)) == rndStart)
        {
          bornY++;
        }
        Assert2.IsTrue(bornY <= level.MaxTileY, "Error: Born Y too large !");
      }
      //debug("bornX = " + bornX);
      //if(playMode == PLAY_AUTO || playMode == PLAY_DEMO || playMode == PLAY_DEMO_ONCE) {
      //    var bornPos = getDemoBornPos();
      //    bornX = bornPos.x;
      //    bornY = bornPos.y;
      //}

      //if(recordMode == RECORD_KEY) saveRecordBornPos(bornX, bornY);
      //else if(recordMode == RECORD_PLAY) {
      //    var bornPos = getRecordBornPos();
      //    bornX = bornPos.x;
      //    bornY = bornPos.y;
      //}

      map[bornX, bornY].Act = CellType.Guard;
      //debug("born (x,y) = (" + bornX + "," + bornY + ")");

      var curGuard = this[id];

      curGuard.Position.Set(bornX, bornY, 0);
      //curGuard.sprite.x = (int)(bornX * level.tileWScale);
      //curGuard.sprite.y = (int)(bornY * level.tileHScale);

      //rebornTimeStart = recordCount;
      //if (AI.curAiVersion < 3)
      //{
      //curGuard.sprite.on("animationend", function() { rebornComplete(id); });
      //curGuard.sprite.gotoAndPlay("reborn");
      //}
      //else
      //{
      add2RebornQueue(id);
      //}

      //curGuard.shape = CellType.Reborn;
      curGuard.Action = LogicActorAction.Reborn;

      _logicModules.Get<LogicViewModule>().GuardReborn(id, bornX, bornY, CellType.Reborn);
    }

    public void add2RebornQueue(int id)
    {
      var curGuard = this[id];

      //curGuard.sprite.gotoAndStop("reborn");
      //curGuard.curFrameIdx  =   0;
      //curGuard.curFrameTime =  -1;

      rebornGuardList.Add(id);
    }

    public bool GuardAlive(int x, int y)
    {
      var i = 0;
      for (; i < guardCount; i++)
      {
        if (this[i].Position.X == x && this[i].Position.Y == y) break;
      }
      Assert2.IsTrue((i < guardCount), "guardAlive() design error !");

      if (this[i].Action != LogicActorAction.Reborn) return true; //alive

      return false; //reborn
    }

    public LogicActorAction bestMove(int id, int playerId)
    {
      var guarder = this[id];
      var x = guarder.Position.X;
      var xOffset = guarder.XOffset;
      var y = guarder.Position.Y;
      var yOffset = guarder.YOffset;
      var map = _logicModules.Get<LogicGridModule>();
      var level = map;
      var runner = _logicModules.Get<LogicPlayerModule>().GetPlayer(playerId);

      CellType curToken, nextBelow;
      LogicActorAction nextMove;
      var checkSameLevelOnly = false;

      curToken = map[x, y].Base;

      if (guarder.Action == LogicActorAction.ClimpOut)
      { //clib from hole
        if (guarder.Position.Y == guarder.SpawnPosition.Y)
        {
          return LogicActorAction.Up;
        }
        else
        {
          checkSameLevelOnly = true;
          if (guarder.Position.X != guarder.SpawnPosition.X)
          { //out of hole
            guarder.Action = LogicActorAction.Left;
          }
        }
      }

      if (!checkSameLevelOnly)
      {

        /****** next check to see if enm must fall. if so ***********/
        /****** return e_fall and skip the rest.          ***********/

        if (curToken == CellType.Ladder || (curToken == CellType.Rope && yOffset == 0))
        { //ladder & bar
          /* no guard fall */
        }
        else if (yOffset < 0) //no laddr & yOffset < 0 ==> falling
          return LogicActorAction.Fall;
        else if (y < level.MaxTileY)
        {
          nextBelow = map[x, y + 1].Act;

          if ((nextBelow == CellType.Empty || nextBelow == CellType.Player))
          {
            return LogicActorAction.Fall;
          }
          else if (nextBelow == CellType.Block || nextBelow == CellType.Solid ||
                  nextBelow == CellType.Guard || nextBelow == CellType.Ladder)
          {
            /* no guard fall */
          }
          else
          {
            return LogicActorAction.Fall;
          }
        }
      }

      /******* next check to see if palyer on same floor *********/
      /******* and whether enm can get him. Ignore walls *********/
      var runnerX = runner.Position.X;
      var runnerY = runner.Position.Y;

      //	if ( y == runnerY ) { // same floor with runner
      if (y == runnerY && runner.Action != LogicActorAction.Fall)
      { //case : guard on laddr and falling => don't catch it 
        while (x != runnerX)
        {
          if (y < level.MaxTileY)
            nextBelow = map[x, y + 1].Base;
          else nextBelow = CellType.Solid;

          curToken = map[x, y].Base;

          if (curToken == CellType.Ladder || curToken == CellType.Rope ||  // go through	
               nextBelow == CellType.Solid || nextBelow == CellType.Ladder ||
               nextBelow == CellType.Block || map[x, y + 1].Act == CellType.Guard || //fixed: must check map[].act with guard_t (for support champLevel:43)
               nextBelow == CellType.Rope || nextBelow == CellType.Gold) //add BAR_T & GOLD_T for support level 92 
          {
            if (x < runnerX)  // guard left to runner
              ++x;
            else if (x > runnerX)
              --x;      // guard right to runner
          }
          else break;     // exit loop with closest x if no path to runner
        }

        if (x == runnerX)  // scan for a path ignoring walls is a success
        {
          if (guarder.Position.X < runnerX)
          {  //if left of man go right else left 
            nextMove = LogicActorAction.Right;
          }
          else if (guarder.Position.Y > runnerX)
          {
            nextMove = LogicActorAction.Left;
          }
          else
          { // guard X = runner X
            if (guarder.XOffset < runner.xOffset)
              nextMove = LogicActorAction.Right;
            else
              nextMove = LogicActorAction.Left;
          }
          return (nextMove);
        }
      } // if ( y == runnerY ) { ... 

      /********** If can't reach man on current level, then scan floor *********/
      /********** (ignoring walls) and look up and down for best move  *********/

      return scanFloor(id, playerId);
    }

    public LogicActorAction scanFloor(int id, int playerId)
    {
      var map = _logicModules.Get<LogicGridModule>();
      var level = map;
      int x, y, startX, startY;
      CellType curToken, nextBelow;


      x = startX = this[id].Position.X;
      y = startY = this[id].Position.Y;

      int bestRating = 255;   // start with worst rating
      int curRating = 255;
      var bestPath = LogicActorAction.Stop;

      /****** get ends for search along floor ******/

      while (x > 0)
      {                                    //get left end first
        curToken = map[x - 1, y].Act;
        if (curToken == CellType.Block || curToken == CellType.Solid)
          break;
        if (curToken == CellType.Ladder || curToken == CellType.Rope || y >= level.MaxTileY ||
             y < level.MaxTileY && ((nextBelow = map[x - 1, y + 1].Base) == CellType.Block ||
             nextBelow == CellType.Solid || nextBelow == CellType.Ladder))
          --x;
        else
        {
          --x;                                        // go on left anyway 
          break;
        }
      }

      var leftEnd = x;
      x = startX;
      while (x < level.MaxTileX)
      {                           // get right end next
        curToken = map[x + 1, y].Act;
        if (curToken == CellType.Block || curToken == CellType.Solid)
          break;

        if (curToken == CellType.Ladder || curToken == CellType.Rope || y >= level.MaxTileY ||
             y < level.MaxTileY && ((nextBelow = map[x + 1, y + 1].Base) == CellType.Block ||
             nextBelow == CellType.Solid || nextBelow == CellType.Ladder))
          ++x;
        else
        {                                         // go on right anyway
          ++x;
          break;
        }
      }

      var rightEnd = x;

      /******* Do middle scan first for best rating and direction *******/

      x = startX;
      if (y < level.MaxTileY &&
           (nextBelow = map[x, y + 1].Base) != CellType.Block && nextBelow != CellType.Solid)
        scanDown(x, LogicActorAction.Down, startX, startY, ref curRating, ref bestRating, ref bestPath, playerId);

      if (map[x, y].Base == CellType.Ladder)
        scanUp(x, LogicActorAction.Up, startX, startY, ref curRating, ref bestRating, ref bestPath, playerId);

      /******* next scan both sides of floor for best rating *******/

      var curPath = LogicActorAction.Left;
      x = leftEnd;

      while (true)
      {
        if (x == startX)
        {
          if (curPath == LogicActorAction.Left && rightEnd != startX)
          {
            curPath = LogicActorAction.Right;
            x = rightEnd;
          }
          else break;
        }

        if (y < level.MaxTileY &&
            (nextBelow = map[x, y + 1].Base) != CellType.Block && nextBelow != CellType.Solid)
          scanDown(x, curPath, startX, startY, ref curRating, ref bestRating, ref bestPath, playerId);

        if (map[x, y].Base == CellType.Ladder)
          scanUp(x, curPath, startX, startY, ref curRating, ref bestRating, ref bestPath, playerId);

        if (curPath == LogicActorAction.Left)
          x++;
        else x--;
      }


      return (bestPath);
    }
    // end scan floor for best direction to go  
    public void scanDown(int x, LogicActorAction curPath, int startX, int startY, ref int curRating, ref int bestRating, ref LogicActorAction bestPath, int playerId)
    {
      var runner = _logicModules.Get<LogicPlayerModule>().GetPlayer(playerId);
      var level = _logicModules.Get<LogicGridModule>();
      var map = level;
      int y;
      CellType nextBelow; //curRating;
      var runnerX = runner.Position.X;
      var runnerY = runner.Position.Y;

      y = startY;

      while (y < level.MaxTileY && (nextBelow = map[x, y + 1].Base) != CellType.Block &&
              nextBelow != CellType.Solid)                  // while no floor below ==> can move down
      {
        if (map[x, y].Base != CellType.Empty && map[x, y].Base != CellType.HLadr)
        { // if not falling ==> try move left or right 
          //************************************************************************************
          // 2014/04/14 Add check  "map[x][y].base != HLADR_T" for support 
          // champLevel 19: a guard in hole with h-ladder can run left after dig the left hole
          //************************************************************************************
          if (x > 0)
          {                          // if not at left edge check left side
            if ((nextBelow = map[x - 1, y + 1].Base) == CellType.Block ||
                nextBelow == CellType.Ladder || nextBelow == CellType.Solid ||
                map[x - 1, y].Base == CellType.Rope)     // can move left       
            {
              if (y >= runnerY)             // no need to go on
                break;                      // already below runner
            }
          }

          if (x < level.MaxTileX)                     // if not at right edge check right side
          {
            if ((nextBelow = map[x + 1, y + 1].Base) == CellType.Block ||
                nextBelow == CellType.Ladder || nextBelow == CellType.Solid ||
                map[x + 1, y].Base == CellType.Rope)     // can move right
            {
              if (y >= runnerY)
                break;
            }
          }
        }                                           // end if not falling
        ++y;                                        // go to next level down
      }                                               // end while ( y < maxTileY ... ) scan down

      if (y == runnerY)
      {                            // update best rating and direct.
        curRating = Math.Abs(startX - x);
        //		if ( (curRating = runnerX - x) < 0) //BUG from original book ? (changed by Simon)
        //			curRating = -curRating; //ABS
      }
      else if (y > runnerY)
        curRating = y - runnerY + 200;               // position below runner
      else curRating = runnerY - y + 100;              // position above runner

      if (curRating < bestRating)
      {
        bestRating = curRating;
        bestPath = curPath;
      }
    }                                                   // end Scan Down

    public void scanUp(int x, LogicActorAction curPath, int startX, int startY, ref int curRating, ref int bestRating, ref LogicActorAction bestPath, int playerId)
    {
      var runner = _logicModules.Get<LogicPlayerModule>().GetPlayer(playerId);
      var level = _logicModules.Get<LogicGridModule>();
      var map = level;
      CellType nextBelow; //curRating;
      var runnerX = runner.Position.X;
      var runnerY = runner.Position.Y;

      var y = startY;

      while (y > 0 && map[x, y].Base == CellType.Ladder)
      {  // while can go up
        --y;
        if (x > 0)
        {                              // if not at left edge check left side
          if ((nextBelow = map[x - 1, y + 1].Base) == CellType.Block ||
              nextBelow == CellType.Solid || nextBelow == CellType.Ladder ||
              map[x - 1, y].Base == CellType.Rope)         // can move left
          {
            if (y <= runnerY)                 // no need to go on 
              break;                          // already above runner
          }
        }

        if (x < level.MaxTileX)
        {                       // if not at right edge check right side
          if ((nextBelow = map[x + 1, y + 1].Base) == CellType.Block ||
              nextBelow == CellType.Solid || nextBelow == CellType.Ladder ||
              map[x + 1, y].Base == CellType.Rope)         // can move right
          {
            if (y <= runnerY)
              break;
          }
        }
        //--y;
      }                                               // end while ( y > 0 && laddr up) scan up 

      if (y == runnerY)
      {                           // update best rating and direct.
        curRating = Math.Abs(startX - x);
        //if ( (curRating = runnerX - x) < 0) // BUG from original book ? (changed by Simon)
        //	curRating = -curRating; //ABS
      }
      else if (y > runnerY)
        curRating = y - runnerY + 200;              // position below runner   
      else curRating = runnerY - y + 100;             // position above runner    

      if (curRating < bestRating)
      {
        bestRating = curRating;
        bestPath = curPath;
      }
    }

    public void guardMoveStep(int id, LogicActorAction action, int playerId)
    {
      var map = _logicModules.Get<LogicGridModule>();
      var level = _logicModules.Get<LogicGridModule>();
      var runner = _logicModules.Get<LogicPlayerModule>().GetPlayer(playerId);
      var yMove = runner.yMove;
      var xMove = runner.xMove;
      var curGuard = this[id];
      var x = curGuard.Position.X;
      var xOffset = curGuard.XOffset;
      var y = curGuard.Position.Y;
      var yOffset = curGuard.YOffset;

      CellType curToken, nextToken;
      LogicActorAction centerX, centerY;
      LogicActorShape curShape, newShape;
      bool stayCurrPos = false;

      centerX = centerY = LogicActorAction.Stop;
      curShape = newShape = curGuard.shape;

      if (curGuard.Action == LogicActorAction.ClimpOut && action == LogicActorAction.Stop)
        curGuard.Action = LogicActorAction.Stop; //for level 16, 30, guard will stock in hole

      switch (action)
      {
        case LogicActorAction.Up:
        case LogicActorAction.Down:
        case LogicActorAction.Fall:
          if (action == LogicActorAction.Up)
          {
            stayCurrPos = (y <= 0 ||
                            (nextToken = map[x, y - 1].Act) == CellType.Block ||
                            nextToken == CellType.Solid || nextToken == CellType.Trap ||
                            nextToken == CellType.Guard);

            if (yOffset <= 0 && stayCurrPos)
              action = LogicActorAction.Stop;
          }
          else
          { //ACT_DOWN || ACT_FALL
            stayCurrPos = (y >= level.MaxTileY ||
                            (nextToken = map[x, y + 1].Act) == CellType.Block ||
                            nextToken == CellType.Solid || nextToken == CellType.Guard);

            if (action == LogicActorAction.Fall && yOffset < 0 && map[x, y].Base == CellType.Block)
            {
              action = LogicActorAction.InHole;
              stayCurrPos = true;
            }
            else
            {
              if (yOffset >= 0 && stayCurrPos)
                action = LogicActorAction.Stop;
            }
          }

          if (action != LogicActorAction.Stop)
          {
            if (xOffset > 0)
              centerX = LogicActorAction.Left;
            else if (xOffset < 0)
              centerX = LogicActorAction.Right;
          }
          break;
        case LogicActorAction.Left:
        case LogicActorAction.Right:
          if (action == LogicActorAction.Left)
          {
            /* original source code from book
            stayCurrPos = ( x <= 0 ||
                            (nextToken = map[x-1][y].act) == BLOCK_T ||
                            nextToken == SOLID_T || nextToken == TRAP_T || 
                            nextToken == GUARD_T); 
            */
            // change check TRAP_T from base, 
            // for support level 41==> runner in trap will cause guard move
            stayCurrPos = (x <= 0 ||
                            (nextToken = map[x - 1, y].Act) == CellType.Block ||
                            nextToken == CellType.Solid || nextToken == CellType.Guard ||
                            map[x - 1, y].Base == CellType.Trap);

            if (xOffset <= 0 && stayCurrPos)
              action = LogicActorAction.Stop;
          }
          else
          { //ACT_RIGHT
            /* original source code from book
            stayCurrPos = ( x >= maxTileX ||
                            (nextToken = map[x+1][y].act) == BLOCK_T ||
                            nextToken == SOLID_T || nextToken == TRAP_T || 
                            nextToken == GUARD_T); 
            */
            // change check TRAP_T from base, 
            // for support level 41==> runner in trap will cause guard move
            stayCurrPos = (x >= level.MaxTileX ||
                            (nextToken = map[x + 1, y].Act) == CellType.Block ||
                            nextToken == CellType.Solid || nextToken == CellType.Guard ||
                            map[x + 1, y].Base == CellType.Trap);

            if (xOffset >= 0 && stayCurrPos)
              action = LogicActorAction.Stop;
          }

          if (action != LogicActorAction.Stop)
          {
            if (yOffset > 0)
              centerY = LogicActorAction.Up;
            else if (yOffset < 0)
              centerY = LogicActorAction.Down;
          }
          break;
      }

      curToken = map[x, y].Base;

      if (action == LogicActorAction.Up)
      {
        yOffset -= yMove;

        if (stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
        else if (yOffset < -level.H2)
        { //move to y-1 position 
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
          y--;
          yOffset = level.TileH + yOffset;
          if (map[x, y].Act == CellType.Player) _logicModules.Get<LogicPlayerModule>().PlayerDead(playerId);//collision
                                                                                                            //map[x][y].act = GUARD_T;
        }

        if (yOffset <= 0 && yOffset > -yMove)
        {
          DropGold(id); //decrease count
        }
        newShape = LogicActorShape.runUpDn;
      }

      if (centerY == LogicActorAction.Up)
      {
        yOffset -= yMove;
        if (yOffset < 0) yOffset = 0; //move to center Y	
      }

      if (action == LogicActorAction.Down || action == LogicActorAction.Fall || action == LogicActorAction.InHole)
      {
        var holdOnBar = 0;
        if (curToken == CellType.Rope)
        {
          if (yOffset < 0) holdOnBar = 1;
          else if (action == LogicActorAction.Down && y < level.MaxTileY && map[x, y + 1].Act != CellType.Ladder)
          {
            action = LogicActorAction.Fall; //shape fixed: 2014/03/27
          }
        }

        yOffset += yMove;

        if (holdOnBar == 1 && yOffset >= 0)
        {
          yOffset = 0; //fall and hold on bar
          action = LogicActorAction.FallBar;
        }
        if (stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
        else if (yOffset > level.H2)
        { //move to y+1 position
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
          y++;
          yOffset = yOffset - level.TileH;
          if (map[x, y].Act == CellType.Player) _logicModules.Get<LogicPlayerModule>().PlayerDead(playerId); //collision
                                                                                                             //map[x][y].act = GUARD_T;
        }

        //add condition: AI version >= 3 will decrease drop count while guard fall
        if (((/*AI.curAiVersion >= 3 &&*/ action == LogicActorAction.Fall) || action == LogicActorAction.Down) &&
             yOffset >= 0 && yOffset < yMove)
        {   //try drop gold
          DropGold(id); //decrease count
        }

        if (action == LogicActorAction.InHole)
        { //check in hole or still falling
          if (yOffset < 0) action = LogicActorAction.Fall; //still falling
          else
          { //fall into hole (yOffset MUST = 0)
            if (curGuard.HasGold > 0)
            {
              if (map[x, y - 1].Base == CellType.Empty)
              {
                //drop gold above
                _logicModules.Get<LogicCoinModule>().AddGold(x, y - 1);
              }
              else
              {
                _logicModules.Get<LogicCoinModule>().DecGold(); //gold disappear 
              }
            }
            curGuard.HasGold = 0;

            if (curShape == LogicActorShape.fallRight) newShape = LogicActorShape.shakeRight;
            else newShape = LogicActorShape.shakeLeft;

            _logicModules.Get<LogicSoundModule>().PlayTrap();

            //shakeTimeStart = recordCount; //for debug

            //if (AI.curAiVersion < 3)
            //{
            //  //curGuard.sprite.on("animationend", function() { climbOut(id); });
            //}
            //else
            //{
            //  //add2GuardShakeQueue(id, newShape);
            //}

            //if(playMode == PLAY_CLASSIC || playMode == PLAY_AUTO || playMode == PLAY_DEMO) {
            //    drawScore(SCORE_IN_HOLE);
            //} else {
            //    //for modem mode & edit mode
            //    //drawGuard(1); //only guard dead need add count
            //}
          }
        }

        if (action == LogicActorAction.Down)
        {
          newShape = LogicActorShape.runUpDn;
        }
        else
        { //ACT_FALL or ACT_FALL_BAR
          if (action == LogicActorAction.FallBar)
          {
            if (curGuard.LastAction == LogicActorAction.Left) newShape = LogicActorShape.barLeft;
            else newShape = LogicActorShape.barRight;
          }
          else
          {
            if (action == LogicActorAction.Fall && curShape != LogicActorShape.fallLeft && curShape != LogicActorShape.fallRight)
            {
              if (curGuard.LastAction == LogicActorAction.Left) newShape = LogicActorShape.fallLeft;
              else newShape = LogicActorShape.fallRight;
            }
          }
        }
      }

      if (centerY == LogicActorAction.Down)
      {
        yOffset += yMove;
        if (yOffset > 0) yOffset = 0; //move to center Y
      }

      if (action == LogicActorAction.Left)
      {
        xOffset -= xMove;

        if (stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
        else if (xOffset < -level.W2)
        { //move to x-1 position 
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
          x--;
          xOffset = level.TileW + xOffset;
          if (map[x, y].Act == CellType.Player) _logicModules.Get<LogicPlayerModule>().PlayerDead(playerId); //collision
                                                                                                             //map[x][y].act = GUARD_T;
        }
        if (xOffset <= 0 && xOffset > -xMove)
        {
          DropGold(id); //try to drop gold
        }
        if (curToken == CellType.Rope) newShape = LogicActorShape.barLeft;
        else newShape = LogicActorShape.runLeft;
      }

      if (centerX == LogicActorAction.Left)
      {
        xOffset -= xMove;
        if (xOffset < 0) xOffset = 0; //move to center X
      }

      if (action == LogicActorAction.Right)
      {
        xOffset += xMove;

        if (stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
        else if (xOffset > level.W2)
        { //move to x+1 position 
          if (curToken == CellType.Block || curToken == CellType.HLadr) curToken = CellType.Empty; //in hole or hide laddr
          map[x, y].Act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
          x++;
          xOffset = xOffset - level.TileW;
          if (map[x, y].Act == CellType.Player) _logicModules.Get<LogicPlayerModule>().PlayerDead(playerId); //collision
                                                                                                             //map[x][y].act = GUARD_T;
        }
        if (xOffset >= 0 && xOffset < xMove)
        {
          DropGold(id);
        }
        if (curToken == CellType.Rope) newShape = LogicActorShape.barRight;
        else newShape = LogicActorShape.runRight;
      }

      if (centerX == LogicActorAction.Right)
      {
        xOffset += xMove;
        if (xOffset > 0) xOffset = 0; //move to center X
      }

      //if(curGuard == ACT_CLIMB_OUT) action == ACT_CLIMB_OUT;

      if (action == LogicActorAction.Stop)
      {
        if (curGuard.Action != LogicActorAction.Stop)
        {
          _logicModules.Get<LogicViewModule>().GuardStop(curGuard.Id);

          if (curGuard.Action != LogicActorAction.ClimpOut) curGuard.Action = LogicActorAction.Stop;
        }
      }
      else
      {
        if (curGuard.Action == LogicActorAction.ClimpOut) action = LogicActorAction.ClimpOut;

        _logicModules.Get<LogicViewModule>().GuardPosition(curGuard.Id, ((x * level.TileW + xOffset)), ((y * level.TileH + yOffset)));

        curGuard.Position.Set(x, y, 0);
        curGuard.SetOffset(xOffset, yOffset);
        if (curShape != newShape)
        {
          _logicModules.Get<LogicViewModule>().GuardShape(newShape);
          curGuard.shape = newShape;
        }
        if (action != curGuard.Action)
        {
          _logicModules.Get<LogicViewModule>().GuardPlay(curGuard.Id);
        }
        curGuard.Action = action;
        if (action == LogicActorAction.Left || action == LogicActorAction.Right) curGuard.LastAction = action;
      }
      map[x, y].Act = CellType.Guard;

      // Check to get gold and carry 
      if (map[x, y].Base == CellType.Gold && curGuard.HasGold == 0 &&
          ((xOffset == 0f && yOffset >= 0 && yOffset < level.H4) ||
           (yOffset == 0f && xOffset >= 0 && xOffset < level.W4) ||
           (y < level.MaxTileY && map[x, y + 1].Base == CellType.Ladder && yOffset < level.H4) // gold above laddr
          )
        )
      {
        //curGuard.hasGold = ((Math.random()*26)+14)|0; //14 - 39 
        curGuard.HasGold = Random.Range(12, 37);//((Math.random()*26)+12)|0; //12 - 37 change gold drop steps

        //if(playMode == PLAY_AUTO || playMode == PLAY_DEMO || playMode == PLAY_DEMO_ONCE) getDemoGold(curGuard);
        //if(recordMode) processRecordGold(curGuard);
        _logicModules.Get<LogicCoinModule>().RemoveGold(x, y);
        //debug ("get, (x,y) = " + x + "," + y + ", offset = " + xOffset); 
      }

      //check collision again 
      //checkCollision(runner.pos.x, runner.pos.y);
    }

    public void add2GuardShakeQueue(int id, LogicActorShape shape)
    {
      //var curGuard = this[id];

      //if(shape == "shakeRight") {
      //    curGuard.shapeFrame = shakeRight;	
      //} else { 
      //    curGuard.shapeFrame = shakeLeft;	
      //}

      //curGuard.curFrameIdx  =  0;
      //curGuard.curFrameTime = -1; //for init

      //shakingGuardList.push(id);
      ////error(arguments.callee.name, "push id =" + id + "(" + shakingGuardList + ")" );

    }


    public void DropGold(int id)
    {
      var curGuard = this[id];
      var map = _logicModules.Get<LogicGridModule>();
      var level = map;
      CellType nextToken;
      var drop = 0;

      if (curGuard.HasGold > 1)
      {
        curGuard.HasGold--; // count > 1,  don't drop it only decrease count 
                            //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
      }
      else if (curGuard.HasGold == 1)
      {
        int x = curGuard.Position.X;
        int y = curGuard.Position.Y;

        if (map[x, y].Base == CellType.Empty && (y >= level.MaxTileY ||
            ((nextToken = map[x, y + 1].Base) == CellType.Block ||
            nextToken == CellType.Solid || nextToken == CellType.Ladder))
        )
        {
          _logicModules.Get<LogicCoinModule>().AddGold(x, y);
          curGuard.HasGold = -1; //for record play action always use value = -1
                                 //curGuard.hasGold =  -(((Math.random()*10)+1)|0); //-1 ~ -10; //waiting time for get gold
          drop = 1;
        }
      }
      else if (curGuard.HasGold < 0)
      {
        curGuard.HasGold++; //wait, don't get gold till count = 0
                            //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
      }
    }

    public LogicGuard this[int id]
    {
      get { return _guards[id]; }
    }

    public int AddGuard(Position position, CellComponent cell)
    {
      _spawnPoints.Add(position);
      var id = Add(new LogicGuard(position));
      _logicModules.Get<LogicViewModule>().AddGuard(id, cell);
      return id;
    }
  }
}