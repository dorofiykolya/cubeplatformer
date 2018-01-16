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

    private readonly int[] _rebornFrame = { 28, 29 };
    private readonly int[] _rebornTime = { 6, 2 };

    private readonly int[] _shakeRight = { 8, 9, 10, 9, 10, 8 };
    private readonly int[] _shakeLeft = { 30, 31, 32, 31, 32, 30 };

    public int MaxGuard;

    public int MoveOffset = 0;
    public int MoveId = 0;
    public int NumOfMoveItems;

    public List<Guard> Guard = new List<Guard>();
    public List<int> RebornGuardList = new List<int>();
    public List<int> ShakingGuardList = new List<int>();

    private Action _bestPath;
    private int _bestRating, _curRating;
    private int _leftEnd, _rightEnd;
    private int _startX, _startY;

    public EngineGuards(Tile[][] map, RandomRange bornX, EngineState state)
    {
      _map = map;
      _bornX = bornX;
      _state = state;
      NumOfMoveItems = state.MovePolicy[0].Length;
    }

    public int GuardCount
    {
      get { return Guard.Count; }
    }

    public void Move()
    {
      if (GuardCount == 0) return; //no guard

      if (++MoveOffset >= NumOfMoveItems) MoveOffset = 0;
      var moves = _state.MovePolicy[GuardCount][MoveOffset];

      while (moves-- > 0)
      {                       // slows guard relative to runner
        if (++MoveId >= GuardCount) MoveId = 0;
        var curGuard = this[MoveId];

        if (curGuard.Action == Action.InHole || curGuard.Action == Action.Reborn)
        {
          continue;
        }

        GuardMoveStep(MoveId, BestMove(MoveId));
      }
    }

    public void GuardMoveStep(int id, Action action)
    {
      var map = _map;
      var curGuard = Guard[id];
      var x = curGuard.Position.X;
      var xOffset = curGuard.Position.XOffset;
      var y = curGuard.Position.Y;
      var yOffset = curGuard.Position.YOffset;

      TileType nextToken;
      Action centerY;
      Shape newShape;
      bool stayCurrPos = false;

      var centerX = centerY = Action.Stop;
      var curShape = newShape = curGuard.Shape;

      if (curGuard.Action == Action.ClimbOut && action == Action.Stop)
      {
        curGuard.Action = Action.Stop; //for level 16, 30, guard will stock in hole
        _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(curGuard.Id, curGuard.Action);
      }

      switch (action)
      {
        case Action.Up:
        case Action.Down:
        case Action.Fall:
          if (action == Action.Up)
          {
            stayCurrPos = (y <= 0 ||
                            (nextToken = map[x][y - 1].Act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T ||
                          nextToken == TileType.GUARD_T);

            if (yOffset <= 0 && stayCurrPos)
              action = Action.Stop;
          }
          else
          { //ACT_DOWN || ACT_FALL
            stayCurrPos = (y >= _state.MaxTileY ||
                            (nextToken = map[x][y + 1].Act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T);

            if (action == Action.Fall && yOffset < 0 && map[x][y].Base == TileType.BLOCK_T)
            {
              action = Action.InHole;
              stayCurrPos = true;
            }
            else
            {
              if (yOffset >= 0 && stayCurrPos)
                action = Action.Stop;
            }
          }

          if (action != Action.Stop)
          {
            if (xOffset > 0)
              centerX = Action.Left;
            else if (xOffset < 0)
              centerX = Action.Right;
          }
          break;
        case Action.Left:
        case Action.Right:
          if (action == Action.Left)
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
                            (nextToken = map[x - 1][y].Act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T ||
                      map[x - 1][y].Base == TileType.TRAP_T);

            if (xOffset <= 0 && stayCurrPos)
              action = Action.Stop;
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
            stayCurrPos = (x >= _state.MaxTileX ||
                            (nextToken = map[x + 1][y].Act) == TileType.BLOCK_T ||
                            nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T ||
                              map[x + 1][y].Base == TileType.TRAP_T);

            if (xOffset >= 0 && stayCurrPos)
              action = Action.Stop;
          }

          if (action != Action.Stop)
          {
            if (yOffset > 0)
              centerY = Action.Up;
            else if (yOffset < 0)
              centerY = Action.Down;
          }
          break;
      }

      var curToken = map[x][y].Base;

      if (action == Action.Up)
      {
        yOffset -= _state.YMove;

        if (stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
        else if (yOffset < -Constants.H2)
        { //move to y-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T || curToken == TileType.TELEPORT_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
          y--;
          yOffset = Constants.TileH + yOffset;
          if (map[x][y].Act == TileType.RUNNER_T) _state.SetRunnerDead(); //collision
                                                                          //map[x][y].act = GUARD_T;
        }

        if (yOffset <= 0 && yOffset > -_state.YMove)
        {
          DropGold(id); //decrease count
        }
        newShape = Shape.RunUp;
      }

      if (centerY == Action.Up)
      {
        yOffset -= _state.YMove;
        if (yOffset < 0) yOffset = 0; //move to center Y	
      }

      if (action == Action.Down || action == Action.Fall || action == Action.InHole)
      {
        var holdOnBar = 0;
        if (curToken == TileType.BAR_T)
        {
          if (yOffset < 0) holdOnBar = 1;
          else if (action == Action.Down && y < _state.MaxTileY && map[x][y + 1].Act != TileType.LADDR_T)
          {
            action = Action.Fall; //shape fixed: 2014/03/27
          }
        }

        yOffset += _state.YMove;

        if (holdOnBar == 1 && yOffset >= 0)
        {
          yOffset = 0; //fall and hold on bar
          action = Action.FallBar;
        }
        if (stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
        else if (yOffset > Constants.H2)
        { //move to y+1 position
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T || curToken == TileType.TELEPORT_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
          y++;
          yOffset = yOffset - Constants.TileH;
          if (map[x][y].Act == TileType.RUNNER_T) _state.SetRunnerDead(); //collision
                                                                          //map[x][y].act = GUARD_T;
        }

        //add condition: AI version >= 3 will decrease drop count while guard fall
        if (((action == Action.Fall) || action == Action.Down) &&
            yOffset >= 0 && yOffset < _state.YMove)
        {   //try drop gold
          DropGold(id); //decrease count
        }

        if (action == Action.InHole)
        { //check in hole or still falling
          if (yOffset < 0)
          {
            action = Action.Fall; //still falling

            //----------------------------------------------------------------------
            //For AI version >= 4, drop gold before guard failing into hole totally
            if (curGuard.HasGold > 0)
            {
              if (map[x][y - 1].Base == TileType.EMPTY_T)
              {
                //drop gold above
                AddGold(x, y - 1);
              }
              else
              {
                DecGold(); //gold disappear 
              }
              curGuard.HasGold = 0;

              GuardRemoveRedhat(curGuard); //9/4/2016
            }
            //----------------------------------------------------------------------

          }
          else
          { //fall into hole (yOffset MUST = 0)

            //----------------------------------------------------------------------
            //For AI version < 4, drop gold after guard failing into hole totally
            if (curGuard.HasGold > 0)
            {
              if (map[x][y - 1].Base == TileType.EMPTY_T)
              {
                //drop gold above
                AddGold(x, y - 1);
              }
              else
                DecGold(); //gold disappear 
            }
            curGuard.HasGold = 0;
            GuardRemoveRedhat(curGuard); //9/4/2016
                                         //----------------------------------------------------------------------

            if (curShape == Shape.FallRight) newShape = Shape.ShakeRight;
            else newShape = Shape.ShakeLeft;
            _state.Sound.LoopSoundPlay(Sounds.Trap);

            Add2GuardShakeQueue(id, newShape);

            _state.DrawScore(Constants.ScoreInHole);

            {
              //for modem mode & edit mode
              //drawGuard(1); //only guard dead need add count
            }
          }
        }

        if (action == Action.Down)
        {
          newShape = Shape.RunDown;
        }
        else
        { //ACT_FALL or ACT_FALL_BAR
          if (action == Action.FallBar)
          {
            if (curGuard.LastLeftRight == Action.Left) newShape = Shape.BarLeft;
            else newShape = Shape.BarRight;
          }
          else
          {
            if (action == Action.Fall && curShape != Shape.FallLeft && curShape != Shape.FallRight)
            {
              if (curGuard.LastLeftRight == Action.Left) newShape = Shape.FallLeft;
              else newShape = Shape.FallRight;
            }
          }
        }
      }

      if (centerY == Action.Down)
      {
        yOffset += _state.YMove;
        if (yOffset > 0) yOffset = 0; //move to center Y
      }

      if (action == Action.Left)
      {
        xOffset -= _state.XMove;

        if (stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
        else if (xOffset < -Constants.W2)
        { //move to x-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T || curToken == TileType.TELEPORT_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
          x--;
          xOffset = Constants.TileW + xOffset;
          if (map[x][y].Act == TileType.RUNNER_T) _state.SetRunnerDead(); //collision
                                                                          //map[x][y].act = GUARD_T;
        }
        if (xOffset <= 0 && xOffset > -_state.XMove)
        {
          DropGold(id); //try to drop gold
        }
        if (curToken == TileType.BAR_T) newShape = Shape.BarLeft;
        else newShape = Shape.RunLeft;
      }

      if (centerX == Action.Left)
      {
        xOffset -= _state.XMove;
        if (xOffset < 0) xOffset = 0; //move to center X
      }

      if (action == Action.Right)
      {
        xOffset += _state.XMove;

        if (stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
        else if (xOffset > Constants.W2)
        { //move to x+1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T || curToken == TileType.TELEPORT_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
          x++;
          xOffset = xOffset - Constants.TileW;
          if (map[x][y].Act == TileType.RUNNER_T) _state.SetRunnerDead(); //collision
                                                                          //map[x][y].act = GUARD_T;
        }
        if (xOffset >= 0 && xOffset < _state.XMove)
        {
          DropGold(id);
        }
        if (curToken == TileType.BAR_T) newShape = Shape.BarRight;
        else newShape = Shape.RunRight;
      }

      if (centerX == Action.Right)
      {
        xOffset += _state.XMove;
        if (xOffset > 0) xOffset = 0; //move to center X
      }

      //if(curGuard == ACT_CLIMB_OUT) action == ACT_CLIMB_OUT;

      if (action == Action.Stop)
      {
        if (curGuard.Action != Action.Stop)
        {
          if (curGuard.Action != Action.ClimbOut)
          {
            curGuard.Action = Action.Stop;
            _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(curGuard.Id, curGuard.Action);
          }
        }
      }
      else
      {
        if (curGuard.Action == Action.ClimbOut) action = Action.ClimbOut;
        curGuard.Position = new Position { X = x, Y = y, XOffset = xOffset, YOffset = yOffset };

        var evt = _state.Output.Enqueue<MoveGuardEvent>(_state.Tick);
        evt.Id = id;
        evt.X = x + xOffset;
        evt.Y = y + yOffset;

        if (curShape != newShape)
        {
          curGuard.Sprite.GotoAndPlay(newShape);
          curGuard.Shape = newShape;

          _state.Output.Enqueue<GuardShapeEvent>(_state.Tick).Set(curGuard.Id, curGuard.Shape);
        }

        curGuard.Action = action;

        _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(curGuard.Id, curGuard.Action);

        if (action == Action.Left || action == Action.Right) curGuard.LastLeftRight = action;
      }
      map[x][y].Act = TileType.GUARD_T;

      // Check to get gold and carry 
      if (map[x][y].Base == TileType.GOLD_T && curGuard.HasGold == 0 &&
         ((Math.Abs(xOffset) > double.Epsilon && yOffset >= 0 && yOffset < Constants.H4) ||
         (Math.Abs(yOffset) > double.Epsilon && xOffset >= 0 && xOffset < Constants.W4) ||
         (y < _state.MaxTileY && map[x][y + 1].Base == TileType.LADDR_T && yOffset < Constants.H4) // gold above laddr
          )
          )
      {
        //curGuard.hasGold = ((Math.random()*26)+14)|0; //14 - 39 
        curGuard.HasGold = (int)((_state.Random() * 26) + 12); //12 - 37 change gold drop steps
        GuardWearRedhat(curGuard); //9/4/2016
        _state.RemoveGold(x, y);
        //debug ("get, (x,y) = " + x + "," + y + ", offset = " + xOffset); 
      }

      //check collision again 
      //checkCollision(runner.pos.x, runner.pos.y);
    }

    private void Add2GuardShakeQueue(int id, Shape shape)
    {
      var curGuard = Guard[id];

      if (shape == Shape.ShakeRight)
      {
        curGuard.ShapeFrame = _shakeRight;
      }
      else
      {
        curGuard.ShapeFrame = _shakeLeft;
      }

      curGuard.CurFrameIdx = 0;
      curGuard.CurFrameTime = -1; //for init

      ShakingGuardList.Add(id);
    }

    private void DecGold()
    {
      _state.DecGold();
    }

    private void AddGold(int x, int y)
    {
      _state.AddGold(x, y);
    }

    private bool DropGold(int id)
    {
      var curGuard = Guard[id];
      var drop = false;
      {
        if (curGuard.HasGold > 1)
        {
          curGuard.HasGold--; // count > 1,  don't drop it only decrease count 
          //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
        }
        else if (curGuard.HasGold == 1)
        {
          //drop gold
          var x = curGuard.Position.X;
          var y = curGuard.Position.Y;

          TileType nextToken;
          if (_map[x][y].Base == TileType.EMPTY_T &&
              (y >= _state.MaxTileY ||
               ((nextToken = _map[x][y + 1].Base) == TileType.BLOCK_T || nextToken == TileType.SOLID_T ||
                nextToken == TileType.LADDR_T)))
          {
            AddGold(x, y);
            curGuard.HasGold = -1; //for record play action always use value = -1
            //curGuard.hasGold =  -(((Math.random()*10)+1)|0); //-1 ~ -10; //waiting time for get gold
            GuardRemoveRedhat(curGuard); //9/4/2016	
            drop = true;
          }
        }
        else if (curGuard.HasGold < 0)
        {
          curGuard.HasGold++; //wait, don't get gold till count = 0
          //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
        }
      }
      return drop;
    }

    public Action BestMove(int id)
    {
      var guarder = Guard[id];
      var x = guarder.Position.X;
      var y = guarder.Position.Y;
      var yOffset = guarder.Position.YOffset;

      TileType nextBelow;
      var checkSameLevelOnly = false;

      var curToken = _map[x][y].Base;

      if (guarder.Action == Action.ClimbOut)
      { //clib from hole
        if (guarder.Position.Y == guarder.HolePos.y)
        {
          return (Action.Up);
        }
        else
        {
          checkSameLevelOnly = true;
          if (guarder.Position.X != guarder.HolePos.x)
          { //out of hole
            guarder.Action = Action.Left;

            _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(guarder.Id, guarder.Action);
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
          return (Action.Fall);
        else if (y < _state.MaxTileY)
        {
          nextBelow = _map[x][y + 1].Act;

          if ((nextBelow == TileType.EMPTY_T || nextBelow == TileType.RUNNER_T || nextBelow == TileType.TELEPORT_T))
          {
            return (Action.Fall);
          }
          else if (nextBelow == TileType.BLOCK_T || nextBelow == TileType.SOLID_T ||
              nextBelow == TileType.GUARD_T || nextBelow == TileType.LADDR_T)
          {
            /* no guard fall */
          }
          else
          {
            return (Action.Fall);
          }
        }
      }

      /******* next check to see if palyer on same floor *********/
      /******* and whether enm can get him. Ignore walls *********/
      var runnerX = _state.Runner.Position.X;
      var runnerY = _state.Runner.Position.Y;

      //	if ( y == runnerY ) { // same floor with runner
      if (y == runnerY && _state.Runner.Action != Action.Fall)
      { //case : guard on laddr and falling => don't catch it 
        while (x != runnerX)
        {
          if (y < _state.MaxTileY)
            nextBelow = _map[x][y + 1].Base;
          else nextBelow = TileType.SOLID_T;

          curToken = _map[x][y].Base;

          if (curToken == TileType.LADDR_T || curToken == TileType.BAR_T ||  // go through	
            nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T || nextBelow == TileType.TELEPORT_T ||
            nextBelow == TileType.BLOCK_T || _map[x][y + 1].Act == TileType.GUARD_T || //fixed: must check map[].act with guard_t (for support champLevel:43)
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
          Action nextMove;
          if (guarder.Position.X < runnerX)
          {  //if left of man go right else left 
            nextMove = Action.Right;
          }
          else if (guarder.Position.X > runnerX)
          {
            nextMove = Action.Left;
          }
          else
          { // guard X = runner X
            if (guarder.Position.XOffset < _state.Runner.Position.XOffset)
              nextMove = Action.Right;
            else
              nextMove = Action.Left;
          }
          return (nextMove);
        }
      } // if ( y == runnerY ) { ... 

      /********** If can't reach man on current level, then scan floor *********/
      /********** (ignoring walls) and look up and down for best move  *********/

      return ScanFloor(id);
    }

    public Action ScanFloor(int id)
    {
      var map = _map;
      var maxTileY = _state.MaxTileY;
      var maxTileX = _state.MaxTileX;
      int x, y;
      TileType curToken, nextBelow;
      Action curPath;

      x = _startX = Guard[id].Position.X;
      y = _startY = Guard[id].Position.Y;

      _bestRating = 255;   // start with worst rating
      _curRating = 255;
      _bestPath = Action.Stop;

      /****** get ends for search along floor ******/

      while (x > 0)
      {                                    //get left end first
        curToken = map[x - 1][y].Act;
        if (curToken == TileType.BLOCK_T || curToken == TileType.SOLID_T)
          break;
        if (curToken == TileType.LADDR_T || curToken == TileType.BAR_T || y >= maxTileY ||
            y < maxTileY && ((nextBelow = map[x - 1][y + 1].Base) == TileType.BLOCK_T ||
              nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T))
          --x;
        else
        {
          --x;                                        // go on left anyway 
          break;
        }
      }

      _leftEnd = x;
      x = _startX;
      while (x < maxTileX)
      {                           // get right end next
        curToken = map[x + 1][y].Act;
        if (curToken == TileType.BLOCK_T || curToken == TileType.SOLID_T)
          break;

        if (curToken == TileType.LADDR_T || curToken == TileType.BAR_T || y >= maxTileY ||
            y < maxTileY && ((nextBelow = map[x + 1][y + 1].Base) == TileType.BLOCK_T ||
              nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T))
          ++x;
        else
        {                                         // go on right anyway
          ++x;
          break;
        }
      }

      _rightEnd = x;

      /******* Do middle scan first for best rating and direction *******/

      x = _startX;
      if (y < maxTileY &&
        (nextBelow = map[x][y + 1].Base) != TileType.BLOCK_T && nextBelow != TileType.SOLID_T)
        ScanDown(x, Action.Down);

      if (map[x][y].Base == TileType.LADDR_T)
        ScanUp(x, Action.Up);

      /******* next scan both sides of floor for best rating *******/

      curPath = Action.Left;
      x = _leftEnd;

      while (true)
      {
        if (x == _startX)
        {
          if (curPath == Action.Left && _rightEnd != _startX)
          {
            curPath = Action.Right;
            x = _rightEnd;
          }
          else break;
        }

        if (y < maxTileY &&
          (nextBelow = map[x][y + 1].Base) != TileType.BLOCK_T && nextBelow != TileType.SOLID_T)
          ScanDown(x, curPath);

        if (map[x][y].Base == TileType.LADDR_T)
          ScanUp(x, curPath);

        if (curPath == Action.Left)
          x++;
        else x--;
      }


      return (_bestPath);
    }                           // end scan floor for best direction to go  

    public void ScanDown(int x, Action curPath)
    {
      var map = _map;
      var maxTileY = _state.MaxTileY;
      var maxTileX = _state.MaxTileX;
      var runner = _state.Runner;
      int y;
      TileType nextBelow; //curRating;
      var runnerY = runner.Position.Y;

      y = _startY;

      while (y < maxTileY && (nextBelow = map[x][y + 1].Base) != TileType.BLOCK_T &&
            nextBelow != TileType.SOLID_T)                  // while no floor below ==> can move down
      {
        if (map[x][y].Base != TileType.EMPTY_T && map[x][y].Base != TileType.HLADR_T && map[x][y].Base != TileType.FINISH_T && map[x][y].Base != TileType.TELEPORT_T)
        { // if not falling ==> try move left or right 
          //************************************************************************************
          // 2014/04/14 Add check  "map[x][y].base != HLADR_T" for support 
          // champLevel 19: a guard in hole with h-ladder can run left after dig the left hole
          //************************************************************************************
          if (x > 0)
          {                          // if not at left edge check left side
            if ((nextBelow = map[x - 1][y + 1].Base) == TileType.BLOCK_T ||
                 nextBelow == TileType.LADDR_T || nextBelow == TileType.SOLID_T ||
                 map[x - 1][y].Base == TileType.BAR_T)     // can move left       
            {
              if (y >= runnerY)             // no need to go on
                break;                      // already below runner
            }
          }

          if (x < maxTileX)                     // if not at right edge check right side
          {
            if ((nextBelow = map[x + 1][y + 1].Base) == TileType.BLOCK_T ||
                 nextBelow == TileType.LADDR_T || nextBelow == TileType.SOLID_T ||
                 map[x + 1][y].Base == TileType.BAR_T)     // can move right
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
        _curRating = Math.Abs(_startX - x);
        //		if ( (curRating = runnerX - x) < 0) //BUG from original book ? (changed by Simon)
        //			curRating = -curRating; //ABS
      }
      else if (y > runnerY)
        _curRating = y - runnerY + 200;               // position below runner
      else _curRating = runnerY - y + 100;              // position above runner

      if (_curRating < _bestRating)
      {
        _bestRating = _curRating;
        _bestPath = curPath;
      }

    }                                                   // end Scan Down

    public void ScanUp(int x, Action curPath)
    {
      var map = _map;
      var maxTileX = _state.MaxTileX;
      var runner = _state.Runner;
      int y;
      var runnerY = runner.Position.Y;

      y = _startY;

      while (y > 0 && map[x][y].Base == TileType.LADDR_T)
      {  // while can go up
        --y;
        TileType nextBelow; //curRating;
        if (x > 0)
        {                              // if not at left edge check left side
          if ((nextBelow = map[x - 1][y + 1].Base) == TileType.BLOCK_T ||
               nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T ||
               map[x - 1][y].Base == TileType.BAR_T)         // can move left
          {
            if (y <= runnerY)                 // no need to go on 
              break;                          // already above runner
          }
        }

        if (x < maxTileX)
        {                       // if not at right edge check right side
          if ((nextBelow = map[x + 1][y + 1].Base) == TileType.BLOCK_T ||
               nextBelow == TileType.SOLID_T || nextBelow == TileType.LADDR_T ||
               map[x + 1][y].Base == TileType.BAR_T)         // can move right
          {
            if (y <= runnerY)
              break;
          }
        }
        //--y;
      }                                               // end while ( y > 0 && laddr up) scan up 

      if (y == runnerY)
      {                           // update best rating and direct.
        _curRating = Math.Abs(_startX - x);
        //if ( (curRating = runnerX - x) < 0) // BUG from original book ? (changed by Simon)
        //	curRating = -curRating; //ABS
      }
      else if (y > runnerY)
        _curRating = y - runnerY + 200;              // position below runner   
      else _curRating = runnerY - y + 100;             // position above runner    

      if (_curRating < _bestRating)
      {
        _bestRating = _curRating;
        _bestPath = curPath;
      }

    }

    public void ProcessGuardShake()
    {
      for (var i = 0; i < ShakingGuardList.Count;)
      {
        var curGuard = Guard[ShakingGuardList[i]];
        var curIdx = curGuard.CurFrameIdx;

        if (curGuard.CurFrameTime < 0)
        { //start shake => set still frame
          curGuard.CurFrameTime = 0;
          curGuard.Sprite.GotoAndStop(curGuard.ShapeFrame[curIdx]);
        }
        else
        {
          if (++curGuard.CurFrameTime >= _state.ShakeTime[curIdx])
          {
            if (++curGuard.CurFrameIdx < curGuard.ShapeFrame.Length)
            {
              //change frame
              curGuard.CurFrameTime = 0;
              curGuard.Sprite.GotoAndStop(curGuard.ShapeFrame[curGuard.CurFrameIdx]);
            }
            else
            {
              //shake time out 

              var id = ShakingGuardList[i];
              ShakingGuardList.RemoveAt(i); //remove from list
                                            //error(arguments.callee.name, "remove id =" + id + "(" + shakingGuardList + ")" );
              ClimbOut(id); //climb out
              continue;
            }

          }
        }
        i++;
      }
    }

    public void ClimbOut(int id)
    {
      var curGuard = Guard[id];
      curGuard.Action = Action.ClimbOut;
      curGuard.Sprite.GotoAndPlay(Shape.RunUp);
      curGuard.Shape = Shape.RunDown;
      curGuard.HolePos = new Point
      {
        x = curGuard.Position.X,
        y = curGuard.Position.Y
      };

      _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(curGuard.Id, curGuard.Action);
      _state.Output.Enqueue<GuardShapeEvent>(_state.Tick).Set(curGuard.Id, curGuard.Shape);
    }

    public void ProcessReborn()
    {
      for (var i = 0; i < RebornGuardList.Count;)
      {
        var curGuard = Guard[RebornGuardList[i]];
        var curIdx = curGuard.CurFrameIdx;

        if (++curGuard.CurFrameTime >= _rebornTime[curIdx])
        {
          if (++curGuard.CurFrameIdx < _rebornFrame.Length)
          {
            //change frame
            curGuard.CurFrameTime = 0;
            curGuard.Sprite.GotoAndStop(_rebornFrame[curGuard.CurFrameIdx]);
          }
          else
          {
            //reborn 
            var id = RebornGuardList[i];
            RebornGuardList.RemoveAt(i); //remove from list
            RebornComplete(id);
            continue;
          }
        }
        i++;
      }
    }

    public int GetGuardId(int x, int y)
    {
      int id;

      for (id = 0; id < GuardCount; id++)
      {
        if (this[id].Position.X == x && this[id].Position.Y == y) break;
      }
      Assert.IsTrue(id < GuardCount, "Error: can not get guard position!");

      return id;
    }

    public Guard this[int id]
    {
      get { return Guard[id]; }
    }

    public bool GuardAlive(int x, int y)
    {
      var i = 0;
      for (; i < GuardCount; i++)
      {
        if (Guard[i].Position.X == x && Guard[i].Position.Y == y) break;
      }
      Assert.IsTrue((i < GuardCount), "guardAlive() design error !");

      if (Guard[i].Action != Action.Reborn) return true; //alive

      return false; //reborn
    }

    public void RemoveFromShake(int id)
    {
      for (var i = 0; i < ShakingGuardList.Count; i++)
      {
        if (ShakingGuardList[i] == id)
        {
          ShakingGuardList.RemoveAt(i); //remove from list
                                        //error(arguments.callee.name, "remove id =" + id + "(" + shakingGuardList + ")" );
          return;
        }
      }
      Assert.IsTrue(false, "design error id =" + id + "(" + ShakingGuardList + ")");
    }

    public void GuardRemoveRedhat(Guard guard)
    {
      var evt = _state.Output.Enqueue<GuardHasGoldEvent>(_state.Tick);
      evt.Id = guard.Id;
      evt.HasGold = false;
    }

    public void GuardWearRedhat(Guard guard)
    {
      var evt = _state.Output.Enqueue<GuardHasGoldEvent>(_state.Tick);
      evt.Id = guard.Id;
      evt.HasGold = true;
    }

    public void GuardReborn(int x, int y)
    {
      var map = _map;
      var bornRndX = _bornX;

      //get guard id  by current in hole position
      var id = GetGuardId(x, y);

      var bornY = 1; //start on line 2
      var bornX = bornRndX.Get();
      var rndStart = bornX;


      while (map[bornX][bornY].Act != TileType.EMPTY_T || map[bornX][bornY].Base == TileType.GOLD_T || map[bornX][bornY].Base == TileType.BLOCK_T)
      {
        //BUG FIXED for level 115 (can not reborn at bornX=27)
        //don't born at gold position & diged position, 2/24/2015
        if ((bornX = bornRndX.Get()) == rndStart)
        {
          bornY++;
        }
        Assert.IsTrue(bornY <= _state.MaxTileY, "Error: Born Y too large !");
      }

      map[bornX][bornY].Act = TileType.GUARD_T;
      //debug("born (x,y) = (" + bornX + "," + bornY + ")");

      var curGuard = Guard[id];

      curGuard.SetTransform(bornX, bornY, 0, 0); //= { x: bornX, y: bornY, xOffset: 0, yOffset: 0 };

      Add2RebornQueue(id);

      curGuard.Shape = Shape.Reborn;
      curGuard.Action = Action.Reborn;

      _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(curGuard.Id, curGuard.Action);
      _state.Output.Enqueue<GuardShapeEvent>(_state.Tick).Set(curGuard.Id, curGuard.Shape);
    }

    public void Add2RebornQueue(int id)
    {
      var curGuard = Guard[id];

      curGuard.Sprite.GotoAndStop(Shape.Reborn);
      curGuard.CurFrameIdx = 0;
      curGuard.CurFrameTime = -1;

      RebornGuardList.Add(id);
    }

    public void RebornComplete(int id)
    {
      var curGuard = Guard[id];
      var x = curGuard.Position.X;
      var y = curGuard.Position.Y;

      if (_map[x][y].Act == TileType.RUNNER_T) _state.SetRunnerDead(); //collision
      _map[x][y].Act = TileType.GUARD_T;
      curGuard.Action = Action.Fall;
      curGuard.Shape = Shape.FallRight;
      //guard[id].hasGold = 0;
      curGuard.Sprite.GotoAndPlay(Shape.FallRight);
      _state.Sound.LoopSoundPlay(Sounds.Reborn);

      _state.Output.Enqueue<GuardActionEvent>(_state.Tick).Set(curGuard.Id, curGuard.Action);
      _state.Output.Enqueue<GuardShapeEvent>(_state.Tick).Set(curGuard.Id, curGuard.Shape);
    }
  }
}
