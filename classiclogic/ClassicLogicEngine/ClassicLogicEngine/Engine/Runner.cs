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
      STATE_NONE = 0,
      STATE_OK_TO_MOVE = 1,
      STATE_FALLING = 2
    }

    public Sprite sprite;
    public Position pos = new Position();
    public Shape shape;
    public Action action;
    private Action lastLeftRight;

    public Runner(Tile[][] map, EngineGuards guards, EngineState state, SpriteSheet spriteSheet)
    {
      _map = map;
      _guards = guards;
      _state = state;

      sprite = new Sprite(spriteSheet);
      sprite.gotoAndPlay(Shape.runRight);
    }

    public void moveRunner()
    {
      var x = pos.x;
      var xOffset = pos.xOffset;
      var y = pos.y;
      var yOffset = pos.yOffset;
      bool stayCurrPos;
      MoveState curState;
      TileType nextToken;

      var curToken = _map[x][y].@base;

      if (curToken == TileType.LADDR_T || (curToken == TileType.BAR_T && yOffset == 0))
      { //ladder & bar
        curState = MoveState.STATE_OK_TO_MOVE; //ok to move (on ladder or bar)
      }
      else if (yOffset < 0)
      {  //no ladder && yOffset < 0 ==> falling 
        curState = MoveState.STATE_FALLING;
      }
      else if (y < Constants.maxTileY)
      { //no laddr && y < maxTileY && yOffset >= 0

        nextToken = _map[x][y + 1].act;

        if (nextToken == TileType.EMPTY_T)
        {
          curState = MoveState.STATE_FALLING;
        }
        else if (nextToken == TileType.BLOCK_T || nextToken == TileType.LADDR_T || nextToken == TileType.SOLID_T)
        {
          curState = MoveState.STATE_OK_TO_MOVE;
        }
        else if (nextToken == TileType.GUARD_T)
        {
          curState = MoveState.STATE_OK_TO_MOVE;
        }
        else
        {
          curState = MoveState.STATE_FALLING;
        }

      }
      else
      { // no laddr && y == maxTileY 
        curState = MoveState.STATE_OK_TO_MOVE;
      }

      if (curState == MoveState.STATE_FALLING)
      {
        stayCurrPos = (y >= Constants.maxTileY || ((nextToken = _map[x][y + 1].act) == TileType.BLOCK_T) || nextToken == TileType.SOLID_T || nextToken == TileType.GUARD_T);

        runnerMoveStep(Action.ACT_FALL, stayCurrPos);
        return;
      }

      /****** Check Key Action ******/

      Action moveStep = Action.ACT_STOP;
      stayCurrPos = true;

      switch (_state.keyAction)
      {
        case Action.ACT_UP:
          stayCurrPos = (y <= 0 ||
            (nextToken = _map[x][y - 1].act) == TileType.BLOCK_T ||
              nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T);

          if (y > 0 && _map[x][y].@base != TileType.LADDR_T && yOffset < Constants.H4 && yOffset > 0 && _map[x][y + 1].@base == TileType.LADDR_T)
          {
            stayCurrPos = true;
            moveStep = Action.ACT_UP;
          }
          else
          if (!(_map[x][y].@base != TileType.LADDR_T &&
           (yOffset <= 0 || _map[x][y + 1].@base != TileType.LADDR_T) ||
             (yOffset <= 0 && stayCurrPos))
          )
          {
            moveStep = Action.ACT_UP;
          }

          break;
        case Action.ACT_DOWN:
          stayCurrPos = (y >= Constants.maxTileY ||
            (nextToken = _map[x][y + 1].act) == TileType.BLOCK_T ||
            nextToken == TileType.SOLID_T);

          if (!(yOffset >= 0 && stayCurrPos))
            moveStep = Action.ACT_DOWN;
          break;
        case Action.ACT_LEFT:
          stayCurrPos = (x <= 0 ||
            (nextToken = _map[x - 1][y].act) == TileType.BLOCK_T ||
            nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T);

          if (!(xOffset <= 0 && stayCurrPos))
            moveStep = Action.ACT_LEFT;
          break;
        case Action.ACT_RIGHT:
          stayCurrPos = (x >= Constants.maxTileX ||
            (nextToken = _map[x + 1][y].act) == TileType.BLOCK_T ||
            nextToken == TileType.SOLID_T || nextToken == TileType.TRAP_T);

          if (!(xOffset >= 0 && stayCurrPos))
            moveStep = Action.ACT_RIGHT;
          break;
        case Action.ACT_DIG_LEFT:
        case Action.ACT_DIG_RIGHT:
          if (ok2Dig(_state.keyAction))
          {
            runnerMoveStep(_state.keyAction, stayCurrPos);
            _state.digHole(_state.keyAction);
          }
          else
          {
            runnerMoveStep(Action.ACT_STOP, stayCurrPos);
          }
          _state.keyAction = Action.ACT_STOP;
          return;
      }
      runnerMoveStep(moveStep, stayCurrPos);
    }

    public void runnerMoveStep(Action currentAction, bool stayCurrPos)
    {
      var map = _map;
      var xMove = _state.xMove;
      var yMove = _state.yMove;
      var tileW = Constants.tileW;
      var tileH = Constants.tileH;
      var H2 = Constants.H2;
      var maxTileY = Constants.maxTileY;

      var x = pos.x;
      var xOffset = pos.xOffset;
      var y = pos.y;
      var yOffset = pos.yOffset;

      Shape newShape;

      var curShape = newShape = this.shape;

      Action centerX = Action.ACT_STOP;
      Action centerY = Action.ACT_STOP;

      switch (currentAction)
      {
        case Action.ACT_DIG_LEFT:
        case Action.ACT_DIG_RIGHT:
          xOffset = 0;
          yOffset = 0;
          break;
        case Action.ACT_UP:
        case Action.ACT_DOWN:
        case Action.ACT_FALL:
          if (xOffset > 0) centerX = Action.ACT_LEFT;
          else if (xOffset < 0) centerX = Action.ACT_RIGHT;
          break;
        case Action.ACT_LEFT:
        case Action.ACT_RIGHT:
          if (yOffset > 0) centerY = Action.ACT_UP;
          else if (yOffset < 0) centerY = Action.ACT_DOWN;
          break;
      }

      var curToken = map[x][y].@base;

      if (currentAction == Action.ACT_UP)
      {
        yOffset -= yMove;

        if (stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
        else if (yOffset < -H2)
        { //move to y-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
          y--;
          yOffset = tileH + yOffset;
          if (map[x][y].act == TileType.GUARD_T && _guards.guardAlive(x, y)) _state.setRunnerDead(); //collision
        }
        newShape = Shape.runUpDn;
      }

      if (centerY == Action.ACT_UP)
      {
        yOffset -= yMove;
        if (yOffset < 0) yOffset = 0; //move to center Y	
      }

      if (currentAction == Action.ACT_DOWN || currentAction == Action.ACT_FALL)
      {
        var holdOnBar = 0;
        if (curToken == TileType.BAR_T)
        {
          if (yOffset < 0) holdOnBar = 1;
          else
          {
            //when runner with bar and press down will into falling state 
            // except "laddr" or "guard" at below, 11/25/2016
            if (currentAction == Action.ACT_DOWN && y < maxTileY &&
              map[x][y + 1].act != TileType.LADDR_T && map[x][y + 1].act != TileType.GUARD_T)
            {
              currentAction = Action.ACT_FALL;
            }
          }
        }

        yOffset += yMove;

        if (holdOnBar == 1 && yOffset >= 0)
        {
          yOffset = 0; //fall and hold on bar
          currentAction = Action.ACT_FALL_BAR;
        }
        if (stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
        else if (yOffset > H2)
        { //move to y+1 position
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
          y++;
          yOffset = yOffset - tileH;
          if (map[x][y].act == TileType.GUARD_T && _guards.guardAlive(x, y)) _state.setRunnerDead(); //collision
        }

        if (currentAction == Action.ACT_DOWN)
        {
          newShape = Shape.runUpDn;
        }
        else
        { //ACT_FALL or ACT_FALL_BAR

          if (y < maxTileY && map[x][y + 1].act == TileType.GUARD_T)
          { //over guard
            //don't collision
            var id = _guards.getGuardId(x, y + 1);
            if (yOffset > _guards[id].pos.yOffset) yOffset = _guards[id].pos.yOffset;
          }

          if (currentAction == Action.ACT_FALL_BAR)
          {
            if (this.lastLeftRight == Action.ACT_LEFT) newShape = Shape.barLeft;
            else newShape = Shape.barRight;
          }
          else
          {
            if (this.lastLeftRight == Action.ACT_LEFT) newShape = Shape.fallLeft;
            else newShape = Shape.fallRight;

          }
        }
      }

      if (centerY == Action.ACT_DOWN)
      {
        yOffset += yMove;
        if (yOffset > 0) yOffset = 0; //move to center Y
      }

      if (currentAction == Action.ACT_LEFT)
      {
        xOffset -= xMove;

        if (stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
        else if (xOffset < -Constants.W2)
        { //move to x-1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
          x--;
          xOffset = tileW + xOffset;
          if (map[x][y].act == TileType.GUARD_T && _guards.guardAlive(x, y)) _state.setRunnerDead(); //collision
        }
        if (curToken == TileType.BAR_T) newShape = Shape.barLeft;
        else newShape = Shape.runLeft;
      }

      if (centerX == Action.ACT_LEFT)
      {
        xOffset -= xMove;
        if (xOffset < 0) xOffset = 0; //move to center X
      }

      if (currentAction == Action.ACT_RIGHT)
      {
        xOffset += xMove;

        if (stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
        else if (xOffset > Constants.W2)
        { //move to x+1 position 
          if (curToken == TileType.BLOCK_T || curToken == TileType.HLADR_T) curToken = TileType.EMPTY_T; //in hole or hide laddr
          map[x][y].act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
          x++;
          xOffset = xOffset - tileW;
          if (map[x][y].act == TileType.GUARD_T && _guards.guardAlive(x, y)) _state.setRunnerDead(); //collision
        }
        if (curToken == TileType.BAR_T) newShape = Shape.barRight;
        else newShape = Shape.runRight;
      }

      if (centerX == Action.ACT_RIGHT)
      {
        xOffset += xMove;
        if (xOffset > 0) xOffset = 0; //move to center X
      }

      if (currentAction == Action.ACT_STOP)
      {
        if (this.action == Action.ACT_FALL)
        {
          _state.Sound.soundStop(Sounds.soundFall);
          _state.Sound.themeSoundPlay(Sounds.down);
        }
        if (this.action != Action.ACT_STOP)
        {
          this.sprite.stop();
          this.action = Action.ACT_STOP;
        }
      }
      else
      {
        sprite.setTransform(x + xOffset, y + yOffset);
        pos = new Position { x = x, y = y, xOffset = xOffset, yOffset = yOffset };

        var evt = _state.Output.Enqueue<MoveRunnerEvent>(_state.Tick);
        evt.x = x + xOffset;
        evt.y = y + yOffset;

        if (curShape != newShape)
        {
          sprite.gotoAndPlay(newShape);
          shape = newShape;
        }
        if (currentAction != this.action)
        {
          if (this.action == Action.ACT_FALL)
          {
            _state.Sound.soundStop(Sounds.soundFall);
            _state.Sound.themeSoundPlay(Sounds.down);
          }
          else if (currentAction == Action.ACT_FALL)
          {
            _state.Sound.soundPlay(Sounds.soundFall);
          }
          this.sprite.play();
        }
        if (currentAction == Action.ACT_LEFT || currentAction == Action.ACT_RIGHT) this.lastLeftRight = currentAction;
        this.action = currentAction;
      }
      map[x][y].act = TileType.RUNNER_T;

      //show trap tile if runner fall into the tile, 9/12/2015
      if (map[x][y].@base == TileType.TRAP_T)
      {
        map[x][y].setAlpha(0.5); //show trap tile

        var evt = _state.Output.Enqueue<ShowTrapEvent>(_state.Tick);
        evt.X = x;
        evt.Y = y;
      }

      // Check runner to get gold (MAX MOVE MUST < H4 & W4) 
      if (map[x][y].@base == TileType.GOLD_T &&
        ((xOffset != 0 && yOffset >= 0 && yOffset < Constants.H4) ||
        (yOffset != 0 && xOffset >= 0 && xOffset < Constants.W4) ||
        (y < maxTileY && map[x][y + 1].@base == TileType.LADDR_T && yOffset < Constants.H4) // gold above laddr
          )
          )
      {
        _state.removeGold(x, y);
        _state.Sound.themeSoundPlay(Sounds.getGold);
        _state.decGold();
        //debug("gold = " + goldCount);
        if (_state.PlayMode == PlayMode.PLAY_CLASSIC || _state.PlayMode == PlayMode.PLAY_AUTO || _state.PlayMode == PlayMode.PLAY_DEMO)
        {
          _state.drawScore(Constants.SCORE_GET_GOLD);
        }
        else
        {
          //for modern mode , edit mode
          _state.drawGold(1); //get gold 
        }
      }
      //if(!goldCount && !goldComplete) showHideLaddr();

      //check collision with guard !
      checkCollision(x, y);
    }

    public void checkCollision(int runnerX, int runnerY)
    {
      var map = _map;
      var x = -1;
      var y = -1;
      //var dbg = "NO";

      if (runnerY > 0 && map[runnerX][runnerY - 1].act == TileType.GUARD_T)
      {
        x = runnerX; y = runnerY - 1;//dbg = "UP";	
      }
      else if (runnerY < Constants.maxTileY && map[runnerX][runnerY + 1].act == TileType.GUARD_T)
      {
        x = runnerX; y = runnerY + 1;//dbg = "DN";	
      }
      else if (runnerX > 0 && map[runnerX - 1][runnerY].act == TileType.GUARD_T)
      {
        x = runnerX - 1; y = runnerY;//dbg = "LF";	
      }
      else if (runnerX < Constants.maxTileX && map[runnerX + 1][runnerY].act == TileType.GUARD_T)
      {
        x = runnerX + 1; y = runnerY;//dbg = "RT";	
      }

      //if( dbg != "NO") debug(dbg);
      if (x >= 0)
      {
        var i = 0;
        for (; i < _guards.guardCount; i++)
        {
          if (_guards[i].pos.x == x && _guards[i].pos.y == y) break;
        }
        Assert.IsTrue((i < _guards.guardCount), "checkCollision design error !");
        if (_guards[i].action != Action.ACT_REBORN)
        { //only guard alive need check collection
          //var dw = Math.abs(runner.sprite.x - guard[i].sprite.x);
          //var dh = Math.abs(runner.sprite.y - guard[i].sprite.y);

          //change detect method ==> don't depend on scale 
          var runnerPosX = this.pos.x * Constants.tileW + this.pos.xOffset;
          var runnerPosY = this.pos.y * Constants.tileH + this.pos.yOffset;
          var guardPosX = _guards[i].pos.x * Constants.tileW + _guards[i].pos.xOffset;
          var guardPosY = _guards[i].pos.y * Constants.tileH + _guards[i].pos.yOffset;

          var dw = Math.Abs(runnerPosX - guardPosX);
          var dh = Math.Abs(runnerPosY - guardPosY);

          if (dw <= Constants.W4 * 3 && dh <= Constants.H4 * 3)
          {
            _state.setRunnerDead(); //07/04/2014
                                    //debug("runner dead!");
          }
        }
      }
    }

    public bool ok2Dig(Action nextMove)
    {
      var x = pos.x;
      var y = pos.y;
      bool result = false;

      switch (nextMove)
      {
        case Action.ACT_DIG_LEFT:
          //		debug("[x-1][y+1] = " + map[x-1][y+1].act + " [x-1][y] = " + map[x-1][y].act + 
          //			  "[x-1][y].base = " + map[x-1][y].base );

          if (y < Constants.maxTileY && x > 0 && _map[x - 1][y + 1].act == TileType.BLOCK_T &&
              _map[x - 1][y].act == TileType.EMPTY_T && _map[x - 1][y].@base != TileType.GOLD_T)
            result = true;
          break;
        case Action.ACT_DIG_RIGHT:
          //		debug("[x+1][y+1] = " + map[x+1][y+1].act + " [x+1][y] = " + map[x+1][y].act + 
          //			  "[x+1][y].base = " + map[x+1][y].base );

          if (y < Constants.maxTileY && x < Constants.maxTileX && _map[x + 1][y + 1].act == TileType.BLOCK_T &&
              _map[x + 1][y].act == TileType.EMPTY_T && _map[x + 1][y].@base != TileType.GOLD_T)
            result = true;
          break;
      }

      return result;
    }

    public void hideRunner()
    {

    }
  }
}
