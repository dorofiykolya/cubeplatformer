using System;
using ClassicLogic.Outputs;
using ClassicLogic.Utils;

namespace ClassicLogic.Engine
{
  public class Runner
  {
    private readonly Tile[][] _map;
    private readonly EngineGuards _guards;
    private readonly EngineState _state;

    public enum MoveState
    {
      StateNone = 0,
      StateOkToMove = 1,
      StateFalling = 2
    }

    public Sprite Sprite;
    public Position Position = new Position();
    public Shape Shape;
    public Action Action;

    private Action _lastLeftRight;
    private bool _teleportEntered;

    public Runner(Tile[][] map, EngineGuards guards, EngineState state)
    {
      _map = map;
      _guards = guards;
      _state = state;

      Sprite = new Sprite();
      Sprite.GotoAndPlay(Shape.RunRight);
    }

    public void Move()
    {
      var x = Position.X;
      var xOffset = Position.XOffset;
      var y = Position.Y;
      var yOffset = Position.YOffset;
      bool stayCurrPos;
      MoveState curState;
      TileType nextToken;

      var curToken = _map[x][y].Base;

      if (curToken == TileType.LADDR_T || (curToken == TileType.BAR_T && Math.Abs(yOffset) <= float.Epsilon))
      { //ladder & bar
        curState = MoveState.StateOkToMove; //ok to move (on ladder or bar)
      }
      else if (yOffset < 0)
      {  //no ladder && yOffset < 0 ==> falling 
        curState = MoveState.StateFalling;
      }
      else if (y < _state.MaxTileY)
      { //no laddr && y < maxTileY && yOffset >= 0

        nextToken = _map[x][y + 1].Act;

        if (nextToken == TileType.EMPTY_T)
        {
          curState = MoveState.StateFalling;
        }
        else if (nextToken == TileType.BLOCK_T || nextToken == TileType.LADDR_T || nextToken == TileType.SOLID_T)
        {
          curState = MoveState.StateOkToMove;
        }
        else if (nextToken == TileType.GUARD_T)
        {
          curState = MoveState.StateOkToMove;
        }
        else
        {
          curState = MoveState.StateFalling;
        }

      }
      else
      { // no laddr && y == maxTileY 
        curState = MoveState.StateOkToMove;
      }

      if (curState == MoveState.StateFalling)
      {
        stayCurrPos = (y >= _state.MaxTileY || ((nextToken = _map[x][y + 1].Act) == TileType.BLOCK_T) || nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T);

        RunnerMoveStep(Action.Fall, stayCurrPos);
        return;
      }

      /****** Check Key Action ******/

      Action moveStep = Action.Stop;
      stayCurrPos = true;

      switch (_state.KeyAction)
      {
        case Action.Up:
          stayCurrPos = (y <= 0 ||
            (nextToken = _map[x][y - 1].Act) == TileType.BLOCK_T ||
              nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T);

          if (y > 0 && _map[x][y].Base != TileType.LADDR_T && yOffset < Constants.H4 && yOffset > 0 && _map[x][y + 1].Base == TileType.LADDR_T)
          {
            stayCurrPos = true;
            moveStep = Action.Up;
          }
          else
          if (!(_map[x][y].Base != TileType.LADDR_T &&
           (yOffset <= 0 || _map[x][y + 1].Base != TileType.LADDR_T) ||
             (yOffset <= 0 && stayCurrPos))
          )
          {
            moveStep = Action.Up;
          }

          break;
        case Action.Down:
          stayCurrPos = (y >= _state.MaxTileY ||
            (nextToken = _map[x][y + 1].Act) == TileType.BLOCK_T ||
            nextToken == TileType.SOLID_T);

          if (!(yOffset >= 0 && stayCurrPos))
            moveStep = Action.Down;
          break;
        case Action.Left:
          stayCurrPos = (x <= 0 ||
            (nextToken = _map[x - 1][y].Act) == TileType.BLOCK_T ||
            nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T);

          if (!(xOffset <= 0 && stayCurrPos))
            moveStep = Action.Left;
          break;
        case Action.Right:
          stayCurrPos = (x >= _state.MaxTileX ||
            (nextToken = _map[x + 1][y].Act) == TileType.BLOCK_T ||
            nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T);

          if (!(xOffset >= 0 && stayCurrPos))
            moveStep = Action.Right;
          break;
        case Action.DigLeft:
        case Action.DigRight:
          if (Ok2Dig(_state.KeyAction))
          {
            RunnerMoveStep(_state.KeyAction, stayCurrPos);
            _state.DigHole(_state.KeyAction);
          }
          else
          {
            RunnerMoveStep(Action.Stop, stayCurrPos);
          }
          _state.KeyAction = Action.Stop;
          return;
      }
      RunnerMoveStep(moveStep, stayCurrPos);
    }

    public void RunnerMoveStep(Action currentAction, bool stayCurrPos)
    {
      var map = _map;
      var xMove = _state.XMove;
      var yMove = _state.YMove;
      var tileW = Constants.TileW;
      var tileH = Constants.TileH;
      const double h2 = Constants.H2;
      var maxTileY = _state.MaxTileY;

      var x = Position.X;
      var xOffset = Position.XOffset;
      var y = Position.Y;
      var yOffset = Position.YOffset;

      Shape newShape;

      var curShape = newShape = Shape;

      Action centerX = Action.Stop;
      Action centerY = Action.Stop;

      switch (currentAction)
      {
        case Action.DigLeft:
        case Action.DigRight:
          xOffset = 0;
          yOffset = 0;
          break;
        case Action.Up:
        case Action.Down:
        case Action.Fall:
          if (xOffset > 0) centerX = Action.Left;
          else if (xOffset < 0) centerX = Action.Right;
          break;
        case Action.Left:
        case Action.Right:
          if (yOffset > 0) centerY = Action.Up;
          else if (yOffset < 0) centerY = Action.Down;
          break;
      }

      var curToken = map[x][y].Base;

      if (currentAction == Action.Up)
      {
        yOffset -= yMove;

        if (stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
        else if (yOffset < -h2)
        { //move to y-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
          y--;
          yOffset = tileH + yOffset;
          if (map[x][y].Act == TileType.GUARD_T && _guards.GuardAlive(x, y)) _state.SetRunnerDead(); //collision
        }
        newShape = Shape.RunUp;
      }

      if (centerY == Action.Up)
      {
        yOffset -= yMove;
        if (yOffset < 0) yOffset = 0; //move to center Y	
      }

      if (currentAction == Action.Down || currentAction == Action.Fall)
      {
        var holdOnBar = 0;
        if (curToken == TileType.BAR_T)
        {
          if (yOffset < 0) holdOnBar = 1;
          else
          {
            //when runner with bar and press down will into falling state 
            // except "laddr" or "guard" at below, 11/25/2016
            if (currentAction == Action.Down && y < maxTileY &&
              map[x][y + 1].Act != TileType.LADDR_T && map[x][y + 1].Act != TileType.GUARD_T)
            {
              currentAction = Action.Fall;
            }
          }
        }

        yOffset += yMove;

        if (holdOnBar == 1 && yOffset >= 0)
        {
          yOffset = 0; //fall and hold on bar
          currentAction = Action.FallBar;
        }
        if (stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
        else if (yOffset > h2)
        { //move to y+1 position
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
          y++;
          yOffset = yOffset - tileH;
          if (map[x][y].Act == TileType.GUARD_T && _guards.GuardAlive(x, y)) _state.SetRunnerDead(); //collision
        }

        if (currentAction == Action.Down)
        {
          newShape = Shape.RunDown;
        }
        else
        { //ACT_FALL or ACT_FALL_BAR

          if (y < maxTileY && map[x][y + 1].Act == TileType.GUARD_T)
          { //over guard
            //don't collision
            var id = _guards.GetGuardId(x, y + 1);
            if (yOffset > _guards[id].Position.YOffset) yOffset = _guards[id].Position.YOffset;
          }

          if (currentAction == Action.FallBar)
          {
            if (_lastLeftRight == Action.Left) newShape = Shape.BarLeft;
            else newShape = Shape.BarRight;
          }
          else
          {
            if (_lastLeftRight == Action.Left) newShape = Shape.FallLeft;
            else newShape = Shape.FallRight;

          }
        }
      }

      if (centerY == Action.Down)
      {
        yOffset += yMove;
        if (yOffset > 0) yOffset = 0; //move to center Y
      }

      if (currentAction == Action.Left)
      {
        xOffset -= xMove;

        if (stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
        else if (xOffset < -Constants.W2)
        { //move to x-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
          x--;
          xOffset = tileW + xOffset;
          if (map[x][y].Act == TileType.GUARD_T && _guards.GuardAlive(x, y)) _state.SetRunnerDead(); //collision
        }
        if (curToken == TileType.BAR_T) newShape = Shape.BarLeft;
        else newShape = Shape.RunLeft;
      }

      if (centerX == Action.Left)
      {
        xOffset -= xMove;
        if (xOffset < 0) xOffset = 0; //move to center X
      }

      if (currentAction == Action.Right)
      {
        xOffset += xMove;

        if (stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
        else if (xOffset > Constants.W2)
        { //move to x+1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T || curToken == TileType.FINISH_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].Act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
          x++;
          xOffset = xOffset - tileW;
          if (map[x][y].Act == TileType.GUARD_T && _guards.GuardAlive(x, y)) _state.SetRunnerDead(); //collision
        }
        if (curToken == TileType.BAR_T) newShape = Shape.BarRight;
        else newShape = Shape.RunRight;
      }

      if (centerX == Action.Right)
      {
        xOffset += xMove;
        if (xOffset > 0) xOffset = 0; //move to center X
      }

      if (currentAction == Action.Stop)
      {
        if (Action == Action.Fall)
        {
          _state.Sound.SoundStop(Sounds.Fall);
          _state.Sound.LoopSoundPlay(Sounds.Down);
        }
        if (Action != Action.Stop)
        {
          _state.Output.Enqueue<RunnerActionEvent>(_state.Tick).Action = Action.Stop;
          Action = Action.Stop;
        }
      }
      else
      {
        Position = new Position { X = x, Y = y, XOffset = xOffset, YOffset = yOffset };

        var evt = _state.Output.Enqueue<MoveRunnerEvent>(_state.Tick);
        evt.x = x + xOffset;
        evt.y = y + yOffset;

        if (curShape != newShape)
        {
          Sprite.GotoAndPlay(newShape);
          Shape = newShape;
          _state.Output.Enqueue<RunnerShapeEvent>(_state.Tick).Shape = Shape;
        }
        if (currentAction != Action)
        {
          if (Action == Action.Fall)
          {
            _state.Sound.SoundStop(Sounds.Fall);
            _state.Sound.LoopSoundPlay(Sounds.Down);
          }
          else if (currentAction == Action.Fall)
          {
            _state.Sound.SoundPlay(Sounds.Fall);
          }
        }
        if (currentAction == Action.Left || currentAction == Action.Right) _lastLeftRight = currentAction;

        Action = currentAction;

        _state.Output.Enqueue<RunnerActionEvent>(_state.Tick).Action = currentAction;
      }
      map[x][y].Act = TileType.RUNNER_T;

      //show trap tile if runner fall into the tile, 9/12/2015
      if (map[x][y].Base == TileType.TRAP_T)
      {
        var evt = _state.Output.Enqueue<ShowTrapEvent>(_state.Tick);
        evt.X = x;
        evt.Y = y;
      }

      // Check runner to get gold (MAX MOVE MUST < H4 & W4) 
      if (map[x][y].Base == TileType.GOLD_T &&
        ((Math.Abs(xOffset) > 0 && yOffset >= 0 && yOffset < Constants.H4) ||
        (Math.Abs(yOffset) > 0 && xOffset >= 0 && xOffset < Constants.W4) ||
        (y < maxTileY && map[x][y + 1].Base == TileType.LADDR_T && yOffset < Constants.H4) // gold above laddr
          )
          )
      {
        _state.RemoveGold(x, y);
        _state.Sound.LoopSoundPlay(Sounds.GetGold);
        _state.DecGold();
        //debug("gold = " + goldCount);

        _state.DrawScore(Constants.ScoreGetGold);

        //for modern mode , edit mode
        _state.DrawGold(1); //get gold 

      }
      //if(!goldCount && !goldComplete) showHideLaddr();

      //check collision with guard !
      CheckTeleport(x, y);
      CheckCollision(x, y);
    }

    public void CheckTeleport(int x, int y)
    {
      if (_map[x][y].Base == TileType.TELEPORT_T)
      {
        if (!_teleportEntered)
        {
          var teleportTo = _state.GetTeleportTo(_map[x][y].Index);
          _map[Position.X][Position.Y].Act = _map[Position.X][Position.Y].Base;
          Position = new Position
          {
            X = teleportTo.x,
            Y = teleportTo.y,
            XOffset = 0,
            YOffset = 0
          };

          _teleportEntered = true;
          _lastLeftRight = Action.Stop;
          Action = Action.Stop;
          _map[Position.X][Position.Y].Act = TileType.RUNNER_T;

          var evt = _state.Output.Enqueue<MoveRunnerEvent>(_state.Tick);
          evt.x = x + Position.XOffset;
          evt.y = y + Position.YOffset;
        }
      }
      else
      {
        _teleportEntered = false;
      }
    }

    public void CheckCollision(int runnerX, int runnerY)
    {
      var map = _map;
      var x = -1;
      var y = -1;
      //var dbg = "NO";

      if (runnerY > 0 && map[runnerX][runnerY - 1].Act == TileType.GUARD_T)
      {
        x = runnerX; y = runnerY - 1;//dbg = "UP";	
      }
      else if (runnerY < _state.MaxTileY && map[runnerX][runnerY + 1].Act == TileType.GUARD_T)
      {
        x = runnerX; y = runnerY + 1;//dbg = "DN";	
      }
      else if (runnerX > 0 && map[runnerX - 1][runnerY].Act == TileType.GUARD_T)
      {
        x = runnerX - 1; y = runnerY;//dbg = "LF";	
      }
      else if (runnerX < _state.MaxTileX && map[runnerX + 1][runnerY].Act == TileType.GUARD_T)
      {
        x = runnerX + 1; y = runnerY;//dbg = "RT";	
      }

      //if( dbg != "NO") debug(dbg);
      if (x >= 0)
      {
        var i = 0;
        for (; i < _guards.GuardCount; i++)
        {
          if (_guards[i].Position.X == x && _guards[i].Position.Y == y) break;
        }
        Assert.IsTrue((i < _guards.GuardCount), "checkCollision design error !");
        if (_guards[i].Action != Action.Reborn)
        { //only guard alive need check collection
          //var dw = Math.abs(runner.sprite.x - guard[i].sprite.x);
          //var dh = Math.abs(runner.sprite.y - guard[i].sprite.y);

          //change detect method ==> don't depend on scale 
          var runnerPosX = Position.X * Constants.TileW + Position.XOffset;
          var runnerPosY = Position.Y * Constants.TileH + Position.YOffset;
          var guardPosX = _guards[i].Position.X * Constants.TileW + _guards[i].Position.XOffset;
          var guardPosY = _guards[i].Position.Y * Constants.TileH + _guards[i].Position.YOffset;

          var dw = Math.Abs(runnerPosX - guardPosX);
          var dh = Math.Abs(runnerPosY - guardPosY);

          if (dw <= Constants.W4 * 3 && dh <= Constants.H4 * 3)
          {
            _state.SetRunnerDead(); //07/04/2014
                                    //debug("runner dead!");
          }
        }
      }
    }

    public bool Ok2Dig(Action nextMove)
    {
      var x = Position.X;
      var y = Position.Y;
      bool result = false;

      switch (nextMove)
      {
        case Action.DigLeft:
          //		debug("[x-1][y+1] = " + map[x-1][y+1].act + " [x-1][y] = " + map[x-1][y].act + 
          //			  "[x-1][y].base = " + map[x-1][y].base );

          if (y < _state.MaxTileY && x > 0 && _map[x - 1][y + 1].Act == TileType.BLOCK_T &&
              _map[x - 1][y].Act == TileType.EMPTY_T && _map[x - 1][y].Base != TileType.GOLD_T)
            result = true;
          break;
        case Action.DigRight:
          //		debug("[x+1][y+1] = " + map[x+1][y+1].act + " [x+1][y] = " + map[x+1][y].act + 
          //			  "[x+1][y].base = " + map[x+1][y].base );

          if (y < _state.MaxTileY && x < _state.MaxTileX && _map[x + 1][y + 1].Act == TileType.BLOCK_T &&
              _map[x + 1][y].Act == TileType.EMPTY_T && _map[x + 1][y].Base != TileType.GOLD_T)
            result = true;
          break;
      }

      return result;
    }
  }
}
