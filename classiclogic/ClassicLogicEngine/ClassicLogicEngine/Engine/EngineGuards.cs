using System;
using System.Collections.Generic;
using ClassicLogic.Outputs;
using ClassicLogic.Utils;

namespace ClassicLogic.Engine
{
  public class EngineGuards
  {
    private readonly Tile[][] _map;
    private readonly RandomRange _bornX;
    private readonly EngineState _state;

    private readonly int[] rebornFrame = { 28, 29 };
    private readonly int[] rebornTime = { 6, 2 };

    private readonly int[] shakeRight = { 8, 9, 10, 9, 10, 8 };
    private readonly int[] shakeLeft = { 30, 31, 32, 31, 32, 30 };

    public int maxGuard;

    public int moveOffset = 0;
    public int moveId = 0;
    public int numOfMoveItems;

    public List<Guard> guard = new List<Guard>();
    public List<int> rebornGuardList = new List<int>();
    public List<int> shakingGuardList = new List<int>();

    public EngineGuards(Tile[][] map, RandomRange bornX, EngineState state)
    {
      _map = map;
      _bornX = bornX;
      _state = state;
      numOfMoveItems = state.movePolicy[0].Length;
    }

    public int guardCount { get { return guard.Count; } }

    public void moveGuard()
    {
      if (guardCount == 0) return; //no guard

      if (++moveOffset >= numOfMoveItems) moveOffset = 0;
      var moves = _state.movePolicy[guardCount][moveOffset];

      while (moves-- > 0)
      {                       // slows guard relative to runner
        if (++moveId >= guardCount) moveId = 0;
        var curGuard = this[moveId];

        if (curGuard.action == Action.ACT_IN_HOLE || curGuard.action == Action.ACT_REBORN)
        {
          continue;
        }

        guardMoveStep(moveId, bestMove(moveId));
      }
    }

    public void guardMoveStep(int id, Action action)
    {
      var map = _map;
      var curGuard = guard[id];
      var x = curGuard.pos.x;
      var xOffset = curGuard.pos.xOffset;
      var y = curGuard.pos.y;
      var yOffset = curGuard.pos.yOffset;

      TileType curToken, nextToken;
      Action centerX, centerY;
      Shape curShape, newShape;
      bool stayCurrPos = false;

      centerX = centerY = Action.ACT_STOP;
      curShape = newShape = curGuard.shape;

      if (curGuard.action == Action.ACT_CLIMB_OUT && action == Action.ACT_STOP)
        curGuard.action = Action.ACT_STOP; //for level 16, 30, guard will stock in hole

      switch (action)
      {
        case Action.ACT_UP:
        case Action.ACT_DOWN:
        case Action.ACT_FALL:
          if (action == Action.ACT_UP)
          {
            stayCurrPos = (y <= 0 ||
                            (nextToken = map[x][y - 1].act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T ||
                          nextToken == TileType.GUARD_T);

            if (yOffset <= 0 && stayCurrPos)
              action = Action.ACT_STOP;
          }
          else
          { //ACT_DOWN || ACT_FALL
            stayCurrPos = (y >= Constants.maxTileY ||
                            (nextToken = map[x][y + 1].act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T);

            if (action == Action.ACT_FALL && yOffset < 0 && map[x][y].@base == TileType.BLOCK_T)
            {
              action = Action.ACT_IN_HOLE;
              stayCurrPos = true;
            }
            else
            {
              if (yOffset >= 0 && stayCurrPos)
                action = Action.ACT_STOP;
            }
          }

          if (action != Action.ACT_STOP)
          {
            if (xOffset > 0)
              centerX = Action.ACT_LEFT;
            else if (xOffset < 0)
              centerX = Action.ACT_RIGHT;
          }
          break;
        case Action.ACT_LEFT:
        case Action.ACT_RIGHT:
          if (action == Action.ACT_LEFT)
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
                            (nextToken = map[x - 1][y].act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T ||
                      map[x - 1][y].@base == TileType.TRAP_T);

            if (xOffset <= 0 && stayCurrPos)
              action = Action.ACT_STOP;
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
            stayCurrPos = (x >= Constants.maxTileX ||
                            (nextToken = map[x + 1][y].act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T ||
                              map[x + 1][y].@base == TileType.TRAP_T);

            if (xOffset >= 0 && stayCurrPos)
              action = Action.ACT_STOP;
          }

          if (action != Action.ACT_STOP)
          {
            if (yOffset > 0)
              centerY = Action.ACT_UP;
            else if (yOffset < 0)
              centerY = Action.ACT_DOWN;
          }
          break;
      }

      curToken = map[x][y].@base;

      if (action == Action.ACT_UP)
      {
        yOffset -= _state.yMove;

        if (stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
        else if (yOffset < -Constants.H2)
        { //move to y-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
          y--;
          yOffset = Constants.tileH + yOffset;
          if (map[x][y].act == TileType.RUNNER_T) _state.setRunnerDead(); //collision
                                                                      //map[x][y].act = GUARD_T;
        }

        if (yOffset <= 0 && yOffset > -_state.yMove)
        {
          dropGold(id); //decrease count
        }
        newShape = Shape.runUpDn;
      }

      if (centerY == Action.ACT_UP)
      {
        yOffset -= _state.yMove;
        if (yOffset < 0) yOffset = 0; //move to center Y	
      }

      if (action == Action.ACT_DOWN || action == Action.ACT_FALL || action == Action.ACT_IN_HOLE)
      {
        var holdOnBar = 0;
        if (curToken == TileType.BAR_T)
        {
          if (yOffset < 0) holdOnBar = 1;
          else if (action == Action.ACT_DOWN && y < Constants.maxTileY && map[x][y + 1].act != TileType.LADDR_T)
          {
            action = Action.ACT_FALL; //shape fixed: 2014/03/27
          }
        }

        yOffset += _state.yMove;

        if (holdOnBar == 1 && yOffset >= 0)
        {
          yOffset = 0; //fall and hold on bar
          action = Action.ACT_FALL_BAR;
        }
        if (stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
        else if (yOffset > Constants.H2)
        { //move to y+1 position
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
          y++;
          yOffset = yOffset - Constants.tileH;
          if (map[x][y].act == TileType.RUNNER_T) _state.setRunnerDead(); //collision
                                                                      //map[x][y].act = GUARD_T;
        }

        //add condition: AI version >= 3 will decrease drop count while guard fall
        if ((((int)_state.AiVersion >= 3 && action == Action.ACT_FALL) || action == Action.ACT_DOWN) &&
            yOffset >= 0 && yOffset < _state.yMove)
        {   //try drop gold
          dropGold(id); //decrease count
        }

        if (action == Action.ACT_IN_HOLE)
        { //check in hole or still falling
          if (yOffset < 0)
          {
            action = Action.ACT_FALL; //still falling

            //----------------------------------------------------------------------
            //For AI version >= 4, drop gold before guard failing into hole totally
            if ((int)_state.AiVersion >= 4 && curGuard.hasGold > 0)
            {
              if (map[x][y - 1].@base == TileType.EMPTY_T)
              {
                //drop gold above
                addGold(x, y - 1);
              }
              else
              {
                decGold(); //gold disappear 
              }
              curGuard.hasGold = 0;
              
              guardRemoveRedhat(curGuard); //9/4/2016
            }
            //----------------------------------------------------------------------

          }
          else
          { //fall into hole (yOffset MUST = 0)

            //----------------------------------------------------------------------
            //For AI version < 4, drop gold after guard failing into hole totally
            if (curGuard.hasGold > 0)
            {
              if (map[x][y - 1].@base == TileType.EMPTY_T)
              {
                //drop gold above
                addGold(x, y - 1);
              }
              else
                decGold(); //gold disappear 
            }
            curGuard.hasGold = 0;
            guardRemoveRedhat(curGuard); //9/4/2016
                                         //----------------------------------------------------------------------

            if (curShape == Shape.fallRight) newShape = Shape.shakeRight;
            else newShape = Shape.shakeLeft;
            _state.Sound.themeSoundPlay(Sounds.trap);
            _state.shakeTimeStart = _state.recordCount; //for debug
            if ((int)_state.AiVersion < 3)
            {
              curGuard.sprite.onAnimationEnded(() => climbOut(id));
            }
            else
            {
              add2GuardShakeQueue(id, newShape);
            }

            if (_state.PlayMode == PlayMode.PLAY_CLASSIC || _state.PlayMode == PlayMode.PLAY_AUTO || _state.PlayMode == PlayMode.PLAY_DEMO)
            {
              _state.drawScore(Constants.SCORE_IN_HOLE);
            }
            else
            {
              //for modem mode & edit mode
              //drawGuard(1); //only guard dead need add count
            }
          }
        }

        if (action == Action.ACT_DOWN)
        {
          newShape = Shape.runUpDn;
        }
        else
        { //ACT_FALL or ACT_FALL_BAR
          if (action == Action.ACT_FALL_BAR)
          {
            if (curGuard.lastLeftRight == Action.ACT_LEFT) newShape = Shape.barLeft;
            else newShape = Shape.barRight;
          }
          else
          {
            if (action == Action.ACT_FALL && curShape != Shape.fallLeft && curShape != Shape.fallRight)
            {
              if (curGuard.lastLeftRight == Action.ACT_LEFT) newShape = Shape.fallLeft;
              else newShape = Shape.fallRight;
            }
          }
        }
      }

      if (centerY == Action.ACT_DOWN)
      {
        yOffset += _state.yMove;
        if (yOffset > 0) yOffset = 0; //move to center Y
      }

      if (action == Action.ACT_LEFT)
      {
        xOffset -= _state.xMove;

        if (stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
        else if (xOffset < -Constants.W2)
        { //move to x-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
          x--;
          xOffset = Constants.tileW + xOffset;
          if (map[x][y].act == TileType.RUNNER_T) _state.setRunnerDead(); //collision
                                                                      //map[x][y].act = GUARD_T;
        }
        if (xOffset <= 0 && xOffset > -_state.xMove)
        {
          dropGold(id); //try to drop gold
        }
        if (curToken == TileType.BAR_T) newShape = Shape.barLeft;
        else newShape = Shape.runLeft;
      }

      if (centerX == Action.ACT_LEFT)
      {
        xOffset -= _state.xMove;
        if (xOffset < 0) xOffset = 0; //move to center X
      }

      if (action == Action.ACT_RIGHT)
      {
        xOffset += _state.xMove;

        if (stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
        else if (xOffset > Constants.W2)
        { //move to x+1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
          x++;
          xOffset = xOffset - Constants.tileW;
          if (map[x][y].act == TileType.RUNNER_T) _state.setRunnerDead(); //collision
                                                                      //map[x][y].act = GUARD_T;
        }
        if (xOffset >= 0 && xOffset < _state.xMove)
        {
          dropGold(id);
        }
        if (curToken == TileType.BAR_T) newShape = Shape.barRight;
        else newShape = Shape.runRight;
      }

      if (centerX == Action.ACT_RIGHT)
      {
        xOffset += _state.xMove;
        if (xOffset > 0) xOffset = 0; //move to center X
      }

      //if(curGuard == ACT_CLIMB_OUT) action == ACT_CLIMB_OUT;

      if (action == Action.ACT_STOP)
      {
        if (curGuard.action != Action.ACT_STOP)
        {
          curGuard.sprite.stop();
          if (curGuard.action != Action.ACT_CLIMB_OUT) curGuard.action = Action.ACT_STOP;
        }
      }
      else
      {
        if (curGuard.action == Action.ACT_CLIMB_OUT) action = Action.ACT_CLIMB_OUT;
        curGuard.sprite.setTransform(x + xOffset, y + yOffset);
        curGuard.pos = new Position { x = x, y = y, xOffset = xOffset, yOffset = yOffset };

        var evt = _state.Output.Enqueue<MoveGuardEvent>(_state.Tick);
        evt.Id = id;
        evt.X = x + xOffset;
        evt.Y = y + yOffset;

        if (curShape != newShape)
        {
          curGuard.sprite.gotoAndPlay(newShape);
          curGuard.shape = newShape;
        }
        if (action != curGuard.action)
        {
          curGuard.sprite.play();
        }
        curGuard.action = action;
        if (action == Action.ACT_LEFT || action == Action.ACT_RIGHT) curGuard.lastLeftRight = action;
      }
      map[x][y].act = TileType.GUARD_T;

      // Check to get gold and carry 
      if (map[x][y].@base == TileType.GOLD_T && curGuard.hasGold == 0 &&
         ((Math.Abs(xOffset) > double.Epsilon && yOffset >= 0 && yOffset < Constants.H4) ||
         (Math.Abs(yOffset) > double.Epsilon && xOffset >= 0 && xOffset < Constants.W4) ||
         (y < Constants.maxTileY && map[x][y + 1].@base == TileType.LADDR_T && yOffset < Constants.H4) // gold above laddr
          )
          )
      {
        //curGuard.hasGold = ((Math.random()*26)+14)|0; //14 - 39 
        curGuard.hasGold = (int)((_state.random() * 26) + 12); //12 - 37 change gold drop steps
        guardWearRedhat(curGuard); //9/4/2016
        if (_state.PlayMode == PlayMode.PLAY_AUTO || _state.PlayMode == PlayMode.PLAY_DEMO || _state.PlayMode == PlayMode.PLAY_DEMO_ONCE) _state.getDemoGold(curGuard);
        if (_state.recordMode != RecordMode.RECORD_NONE) _state.processRecordGold(curGuard);
        _state.removeGold(x, y);
        //debug ("get, (x,y) = " + x + "," + y + ", offset = " + xOffset); 
      }

      //check collision again 
      //checkCollision(runner.pos.x, runner.pos.y);
    }

    private void add2GuardShakeQueue(int id, Shape shape)
    {
      var curGuard = guard[id];

      if (shape == Shape.shakeRight)
      {
        curGuard.shapeFrame = shakeRight;
      }
      else
      {
        curGuard.shapeFrame = shakeLeft;
      }

      curGuard.curFrameIdx = 0;
      curGuard.curFrameTime = -1; //for init

      shakingGuardList.Add(id);
    }

    private void decGold()
    {
      _state.decGold();
    }

    private void addGold(int x, int y)
    {
      _state.addGold(x, y);
    }

    private bool dropGold(int id)
    {
      var curGuard = guard[id];
      var drop = false;
      {
        if (curGuard.hasGold > 1)
        {
          curGuard.hasGold--; // count > 1,  don't drop it only decrease count 
          //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
        }
        else if (curGuard.hasGold == 1)
        {
          //drop gold
          var x = curGuard.pos.x;
          var y = curGuard.pos.y;

          TileType nextToken;
          if (_map[x][y].@base == TileType.EMPTY_T &&
              (y >= Constants.maxTileY ||
               ((nextToken = _map[x][y + 1].@base) == TileType.BLOCK_T || nextToken == TileType.SOLID_T ||
                nextToken == TileType.LADDR_T)))
          {
            addGold(x, y);
            curGuard.hasGold = -1; //for record play action always use value = -1
            //curGuard.hasGold =  -(((Math.random()*10)+1)|0); //-1 ~ -10; //waiting time for get gold
            guardRemoveRedhat(curGuard); //9/4/2016	
            drop = true;
          }
        }
        else if (curGuard.hasGold < 0)
        {
          curGuard.hasGold++; //wait, don't get gold till count = 0
          //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
        }
      }
      return drop;
    }

    public Action bestMove(int id)
    {
      var guarder = guard[id];
      var x = guarder.pos.x;
      var xOffset = guarder.pos.xOffset;
      var y = guarder.pos.y;
      var yOffset = guarder.pos.yOffset;

      TileType curToken, nextBelow;
      Action nextMove;
      var checkSameLevelOnly = false;

      curToken = _map[x][y].@base;

      if (guarder.action == Action.ACT_CLIMB_OUT)
      { //clib from hole
        if (guarder.pos.y == guarder.holePos.y)
        {
          return (Action.ACT_UP);
        }
        else
        {
          checkSameLevelOnly = true;
          if (guarder.pos.x != guarder.holePos.x)
          { //out of hole
            guarder.action = Action.ACT_LEFT;
          }
        }
      }

      if (!checkSameLevelOnly)
      {

        /****** next check to see if enm must fall. if so ***********/
        /****** return e_fall and skip the rest.          ***********/

        if (curToken == TileType.LADDR_T || (curToken == TileType.BAR_T && Math.Abs(yOffset) < double.Epsilon))
        { //ladder & bar
          /* no guard fall */
        }
        else if (yOffset < 0) //no laddr & yOffset < 0 ==> falling
          return (Action.ACT_FALL);
        else if (y < Constants.maxTileY)
        {
          nextBelow = _map[x][y + 1].act;

          if ((nextBelow == TileType.EMPTY_T || nextBelow == TileType.RUNNER_T))
          {
            return (Action.ACT_FALL);
          }
          else if (nextBelow == TileType.BLOCK_T || nextBelow == TileType.SOLID_T ||
              nextBelow == TileType.GUARD_T || nextBelow == TileType.LADDR_T)
          {
            /* no guard fall */
          }
          else
          {
            return (Action.ACT_FALL);
          }
        }
      }

      /******* next check to see if palyer on same floor *********/
      /******* and whether enm can get him. Ignore walls *********/
      var runnerX = _state.runner.pos.x;
      var runnerY = _state.runner.pos.y;

      //	if ( y == runnerY ) { // same floor with runner
      if (y == runnerY && _state.runner.action != Action.ACT_FALL)
      { //case : guard on laddr and falling => don't catch it 
        while (x != runnerX)
        {
          if (y < Constants.maxTileY)
            nextBelow = _map[x][y + 1].@base;
          else nextBelow = TileType.SOLID_T;

          curToken = _map[x][y].@base;

          if (curToken == TileType.LADDR_T || curToken == TileType.BAR_T ||  // go through	
            nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T ||
            nextBelow == TileType.BLOCK_T || _map[x][y + 1].act == TileType.GUARD_T || //fixed: must check map[].act with guard_t (for support champLevel:43)
              nextBelow == TileType.BAR_T || nextBelow == TileType.GOLD_T) //add BAR_T & GOLD_T for support level 92 
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
          if (guarder.pos.x < runnerX)
          {  //if left of man go right else left 
            nextMove = Action.ACT_RIGHT;
          }
          else if (guarder.pos.x > runnerX)
          {
            nextMove = Action.ACT_LEFT;
          }
          else
          { // guard X = runner X
            if (guarder.pos.xOffset < _state.runner.pos.xOffset)
              nextMove = Action.ACT_RIGHT;
            else
              nextMove = Action.ACT_LEFT;
          }
          return (nextMove);
        }
      } // if ( y == runnerY ) { ... 

      /********** If can't reach man on current level, then scan floor *********/
      /********** (ignoring walls) and look up and down for best move  *********/

      return scanFloor(id);
    }

    private Action bestPath;
    int bestRating, curRating;
    int leftEnd, rightEnd;
    int startX, startY;

    public Action scanFloor(int id)
    {
      var map = _map;
      var maxTileY = Constants.maxTileY;
      var maxTileX = Constants.maxTileX;
      int x, y;
      TileType curToken, nextBelow;
      Action curPath;

      x = startX = guard[id].pos.x;
      y = startY = guard[id].pos.y;

      bestRating = 255;   // start with worst rating
      curRating = 255;
      bestPath = Action.ACT_STOP;

      /****** get ends for search along floor ******/

      while (x > 0)
      {                                    //get left end first
        curToken = map[x - 1][y].act;
        if (curToken == TileType.BLOCK_T || curToken == TileType.SOLID_T)
          break;
        if (curToken == TileType.LADDR_T || curToken == TileType.BAR_T || y >= maxTileY ||
            y < maxTileY && ((nextBelow = map[x - 1][y + 1].@base) == TileType.BLOCK_T ||
              nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T))
          --x;
        else
        {
          --x;                                        // go on left anyway 
          break;
        }
      }

      leftEnd = x;
      x = startX;
      while (x < maxTileX)
      {                           // get right end next
        curToken = map[x + 1][y].act;
        if (curToken == TileType.BLOCK_T || curToken == TileType.SOLID_T)
          break;

        if (curToken == TileType.LADDR_T || curToken == TileType.BAR_T || y >= maxTileY ||
            y < maxTileY && ((nextBelow = map[x + 1][y + 1].@base) == TileType.BLOCK_T ||
              nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T))
          ++x;
        else
        {                                         // go on right anyway
          ++x;
          break;
        }
      }

      rightEnd = x;

      /******* Do middle scan first for best rating and direction *******/

      x = startX;
      if (y < maxTileY &&
        (nextBelow = map[x][y + 1].@base) != TileType.BLOCK_T && nextBelow != TileType.SOLID_T)
        scanDown(x, Action.ACT_DOWN);

      if (map[x][y].@base == TileType.LADDR_T)
        scanUp(x, Action.ACT_UP);

      /******* next scan both sides of floor for best rating *******/

      curPath = Action.ACT_LEFT;
      x = leftEnd;

      while (true)
      {
        if (x == startX)
        {
          if (curPath == Action.ACT_LEFT && rightEnd != startX)
          {
            curPath = Action.ACT_RIGHT;
            x = rightEnd;
          }
          else break;
        }

        if (y < maxTileY &&
          (nextBelow = map[x][y + 1].@base) != TileType.BLOCK_T && nextBelow != TileType.SOLID_T)
          scanDown(x, curPath);

        if (map[x][y].@base == TileType.LADDR_T)
          scanUp(x, curPath);

        if (curPath == Action.ACT_LEFT)
          x++;
        else x--;
      }


      return (bestPath);
    }                           // end scan floor for best direction to go  

    public void scanDown(int x, Action curPath)
    {
      var map = _map;
      var maxTileY = Constants.maxTileY;
      var maxTileX = Constants.maxTileX;
      var runner = _state.runner;
      int y;
      TileType nextBelow; //curRating;
      var runnerX = runner.pos.x;
      var runnerY = runner.pos.y;

      y = startY;

      while (y < maxTileY && (nextBelow = map[x][y + 1].@base) != TileType.BLOCK_T &&
            nextBelow != TileType.SOLID_T)                  // while no floor below ==> can move down
      {
        if (map[x][y].@base != TileType.EMPTY_T && map[x][y].@base != TileType.HLADR_T)
        { // if not falling ==> try move left or right 
          //************************************************************************************
          // 2014/04/14 Add check  "map[x][y].base != HLADR_T" for support 
          // champLevel 19: a guard in hole with h-ladder can run left after dig the left hole
          //************************************************************************************
          if (x > 0)
          {                          // if not at left edge check left side
            if ((nextBelow = map[x - 1][y + 1].@base) == TileType.BLOCK_T ||
                 nextBelow == TileType.LADDR_T || nextBelow == TileType.SOLID_T ||
                 map[x - 1][y].@base == TileType.BAR_T)     // can move left       
            {
              if (y >= runnerY)             // no need to go on
                break;                      // already below runner
            }
          }

          if (x < maxTileX)                     // if not at right edge check right side
          {
            if ((nextBelow = map[x + 1][y + 1].@base) == TileType.BLOCK_T ||
                 nextBelow == TileType.LADDR_T || nextBelow == TileType.SOLID_T ||
                 map[x + 1][y].@base == TileType.BAR_T)     // can move right
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

    public void scanUp(int x, Action curPath)
    {
      var map = _map;
      var maxTileX = Constants.maxTileX;
      var runner = _state.runner;
      int y;
      var runnerX = runner.pos.x;
      var runnerY = runner.pos.y;

      y = startY;

      while (y > 0 && map[x][y].@base == TileType.LADDR_T)
      {  // while can go up
        --y;
        TileType nextBelow; //curRating;
        if (x > 0)
        {                              // if not at left edge check left side
          if ((nextBelow = map[x - 1][y + 1].@base) == TileType.BLOCK_T ||
               nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T ||
               map[x - 1][y].@base == TileType.BAR_T)         // can move left
          {
            if (y <= runnerY)                 // no need to go on 
              break;                          // already above runner
          }
        }

        if (x < maxTileX)
        {                       // if not at right edge check right side
          if ((nextBelow = map[x + 1][y + 1].@base) == TileType.BLOCK_T ||
               nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T ||
               map[x + 1][y].@base == TileType.BAR_T)         // can move right
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

    public void processGuardShake()
    {
      for (var i = 0; i < shakingGuardList.Count;)
      {
        var curGuard = guard[shakingGuardList[i]];
        var curIdx = curGuard.curFrameIdx;

        if (curGuard.curFrameTime < 0)
        { //start shake => set still frame
          curGuard.curFrameTime = 0;
          curGuard.sprite.gotoAndStop(curGuard.shapeFrame[curIdx]);
        }
        else
        {
          if (++curGuard.curFrameTime >= _state.shakeTime[curIdx])
          {
            if (++curGuard.curFrameIdx < curGuard.shapeFrame.Length)
            {
              //change frame
              curGuard.curFrameTime = 0;
              curGuard.sprite.gotoAndStop(curGuard.shapeFrame[curGuard.curFrameIdx]);
            }
            else
            {
              //shake time out 

              var id = shakingGuardList[i];
              shakingGuardList.RemoveAt(i); //remove from list
                                            //error(arguments.callee.name, "remove id =" + id + "(" + shakingGuardList + ")" );
              climbOut(id); //climb out
              continue;
            }

          }
        }
        i++;
      }
    }

    public void climbOut(int id)
    {
      var curGuard = guard[id];
      curGuard.action = Action.ACT_CLIMB_OUT;
      curGuard.sprite.removeAllEventListeners();
      curGuard.sprite.gotoAndPlay(Shape.runUpDn);
      curGuard.shape = Shape.runUpDn;
      curGuard.holePos = new Point
      {
        x = curGuard.pos.x,
        y = curGuard.pos.y
      };
    }

    public void processReborn()
    {
      for (var i = 0; i < rebornGuardList.Count;)
      {
        var curGuard = guard[rebornGuardList[i]];
        var curIdx = curGuard.curFrameIdx;

        if (++curGuard.curFrameTime >= rebornTime[curIdx])
        {
          if (++curGuard.curFrameIdx < rebornFrame.Length)
          {
            //change frame
            curGuard.curFrameTime = 0;
            curGuard.sprite.gotoAndStop(rebornFrame[curGuard.curFrameIdx]);
          }
          else
          {
            //reborn 
            var id = rebornGuardList[i];
            rebornGuardList.RemoveAt(i); //remove from list
            rebornComplete(id);
            continue;
          }
        }
        i++;
      }
    }

    public int getGuardId(int x, int y)
    {
      int id;

      for (id = 0; id < guardCount; id++)
      {
        if (this[id].pos.x == x && this[id].pos.y == y) break;
      }
      Assert.IsTrue(id < guardCount, "Error: can not get guard position!");

      return id;
    }

    public Guard this[int id]
    {
      get { return guard[id]; }
    }

    public bool guardAlive(int x, int y)
    {
      var i = 0;
      for (; i < guardCount; i++)
      {
        if (guard[i].pos.x == x && guard[i].pos.y == y) break;
      }
      Assert.IsTrue((i < guardCount), "guardAlive() design error !");

      if (guard[i].action != Action.ACT_REBORN) return true; //alive

      return false; //reborn
    }

    public void removeFromShake(int id)
    {
      for (var i = 0; i < shakingGuardList.Count; i++)
      {
        if (shakingGuardList[i] == id)
        {
          shakingGuardList.RemoveAt(i); //remove from list
                                        //error(arguments.callee.name, "remove id =" + id + "(" + shakingGuardList + ")" );
          return;
        }
      }
      Assert.IsTrue(false, "design error id =" + id + "(" + shakingGuardList + ")");
    }

    public void guardRemoveRedhat(Guard guard)
    {
      var evt = _state.Output.Enqueue<GuardHasGoldEvent>(_state.Tick);
      evt.Id = guard.Id;
      evt.HasGold = false;
    }

    public void guardWearRedhat(Guard guard)
    {

      var evt = _state.Output.Enqueue<GuardHasGoldEvent>(_state.Tick);
      evt.Id = guard.Id;
      evt.HasGold = true;
    }

    public void guardReborn(int x, int y)
    {
      var map = _map;
      var bornRndX = _bornX;

      //get guard id  by current in hole position
      var id = getGuardId(x, y);

      var bornY = 1; //start on line 2
      var bornX = bornRndX.get();
      var rndStart = bornX;


      while (map[bornX][bornY].act != TileType.EMPTY_T || map[bornX][bornY].@base == TileType.GOLD_T || map[bornX][bornY].@base == TileType.BLOCK_T)
      {
        //BUG FIXED for level 115 (can not reborn at bornX=27)
        //don't born at gold position & diged position, 2/24/2015
        if ((bornX = bornRndX.get()) == rndStart)
        {
          bornY++;
        }
        Assert.IsTrue(bornY <= Constants.maxTileY, "Error: Born Y too large !");
      }
      //debug("bornX = " + bornX);
      if (_state.PlayMode == PlayMode.PLAY_AUTO || _state.PlayMode == PlayMode.PLAY_DEMO || _state.PlayMode == PlayMode.PLAY_DEMO_ONCE)
      {
        var bornPos = _state.getDemoBornPos();
        bornX = bornPos.x;
        bornY = bornPos.y;
      }

      if (_state.recordMode == RecordMode.RECORD_KEY) _state.saveRecordBornPos(bornX, bornY);
      else if (_state.recordMode == RecordMode.RECORD_PLAY)
      {
        var bornPos = _state.getRecordBornPos();
        bornX = bornPos.x;
        bornY = bornPos.y;
      }

      map[bornX][bornY].act = TileType.GUARD_T;
      //debug("born (x,y) = (" + bornX + "," + bornY + ")");

      var curGuard = guard[id];

      curGuard.setTransform(bornX, bornY, 0, 0); //= { x: bornX, y: bornY, xOffset: 0, yOffset: 0 };
      curGuard.sprite.setTransform(bornX, bornY);

      _state.rebornTimeStart = _state.recordCount;
      if ((int)_state.AiVersion < 3)
      {
        curGuard.sprite.onAnimationEnded(() => rebornComplete(id));
        curGuard.sprite.gotoAndPlay(Shape.reborn);
      }
      else
      {
        add2RebornQueue(id);
      }

      curGuard.shape = Shape.reborn;
      curGuard.action = Action.ACT_REBORN;

    }

    public void add2RebornQueue(int id)
    {
      var curGuard = guard[id];

      curGuard.sprite.gotoAndStop(Shape.reborn);
      curGuard.curFrameIdx = 0;
      curGuard.curFrameTime = -1;

      rebornGuardList.Add(id);
    }

    public void rebornComplete(int id)
    {
      var x = guard[id].pos.x;
      var y = guard[id].pos.y;

      if (_map[x][y].act == TileType.RUNNER_T) _state.setRunnerDead(); //collision
      _map[x][y].act = TileType.GUARD_T;
      guard[id].sprite.removeAllEventListeners();
      guard[id].action = Action.ACT_FALL;
      guard[id].shape = Shape.fallRight;
      //guard[id].hasGold = 0;
      guard[id].sprite.gotoAndPlay(Shape.fallRight);
      _state.Sound.themeSoundPlay(Sounds.reborn);
    }
  }
}
