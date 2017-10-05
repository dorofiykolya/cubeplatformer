using System;
using System.Collections.Generic;
using System.Linq;
using ClassicLogic.Outputs;
using ClassicLogic.Utils;

namespace ClassicLogic.Engine
{
  public class EngineState
  {
    private readonly LevelMap _levelMap;
    private readonly Engine _engine;
    private readonly AIVersion _aiVersion;
    private readonly Configuration _config;
    private readonly Random _random;
    private readonly List<Sprite> _sprites;

    public readonly double xMove;
    public readonly double yMove;

    public int playTickTimer;
    public Action keyAction;
    public int goldCount;
    public Runner runner;
    public bool goldComplete;
    public int curTime;
    public RecordMode recordMode;
    public EngineGuards guards;
    public HoleObj holeObj;
    public Tile[][] map;
    public RandomRange bornX;

    public int digTimeStart;
    public int recordCount;
    public List<Sprite> fillHoleObj = new List<Sprite>();
    public int fillHoleTimeStart;
    private bool godMode;

    private readonly int[] digHoleLeft = { 0, 1, 2, 2, 3, 4, 4, 5, 6, 6, 7 };
    private readonly int[] digHoleRight = { 8, 9, 10, 10, 11, 12, 12, 13, 14, 14, 15 };
    private readonly int[] fillHoleFrame = { 16, 17, 18, 19 };
    private readonly int[] fillHoleTime = { 166, 8, 8, 4 };
    public int rebornTimeStart;
    public int[] shakeTime;
    public int[][] movePolicy;
    public int shakeTimeStart;

    public EngineState(AIVersion aiVersion, Mode mode, Configuration config, LevelMap levelMap, Engine engine)
    {
      _aiVersion = aiVersion;
      _config = config;
      _levelMap = levelMap;
      _engine = engine;

      _random = new Random((int)aiVersion);
      _sprites = new List<Sprite>();

      xMove = config.xMoveBase;
      yMove = config.yMoveBase;

      map = _levelMap.ToArray();
      goldCount = levelMap.goldCount;

      PlayMode = mode == Mode.Classic ? PlayMode.PLAY_CLASSIC : PlayMode.PLAY_MODERN;

      movePolicy = config.movePolicy;
      shakeTime = config.shakeTime;
      bornX = new RandomRange(0, Constants.maxTileX, 0);
      guards = new EngineGuards(map, bornX, this);
      guards.maxGuard = _levelMap.maxGuard;

      holeObj = new HoleObj(map, this, config.createHole());
      runner = new Runner(map, guards, this, config.createRunner());

      _sprites.Add(runner.sprite);

      for (int x = 0; x < _levelMap.XCount; x++)
      {
        for (int y = 0; y < _levelMap.YCount; y++)
        {
          var tile = _levelMap[x][y];
          if (tile.act == TileType.GUARD_T)
          {
            var curGuard = new Guard(x, y, guards.guardCount, config.createGuard());
            _sprites.Add(curGuard.sprite);
            guards.guard.Add(curGuard);
          }
          else if (tile.act == TileType.RUNNER_T)
          {
            runner.pos.x = x;
            runner.pos.y = y;
          }
        }
      }

      var evt = engine.Output.Enqueue<InitializeEvent>(0);
      evt.Map = map.Select(i => i.Select(a => a.@base).ToArray()).ToArray();
      evt.Guard = guards.guard.Select(g => new InitializeEvent.GuardData
      {
        Position = new Point
        {
          x = g.pos.x,
          y = g.pos.y
        },
        Id = g.Id
      }).ToArray();
      evt.Runner = new Point
      {
        x = runner.pos.x,
        y = runner.pos.y
      };
    }

    public GameState State { get; set; }
    public PlayMode PlayMode { get; set; }
    public RecordMode RecordMode { get; set; }
    public int Tick { get; set; }
    public EngineSound Sound { get { return _engine.Sound; } }
    public EngineOutput Output { get { return _engine.Output; } }
    public AIVersion AiVersion { get { return _engine.AiVersion; } }

    public void pressKey(KeyCode code)
    {
      switch (code)
      {
        case KeyCode.KEYCODE_LEFT:
        case KeyCode.KEYCODE_J:
        case KeyCode.KEYCODE_A:
          keyAction = Action.ACT_LEFT;
          break;
        case KeyCode.KEYCODE_RIGHT:
        case KeyCode.KEYCODE_L:
        case KeyCode.KEYCODE_D:
          keyAction = Action.ACT_RIGHT;
          break;
        case KeyCode.KEYCODE_UP:
        case KeyCode.KEYCODE_I:
        case KeyCode.KEYCODE_W:
          keyAction = Action.ACT_UP;
          break;
        case KeyCode.KEYCODE_DOWN:
        case KeyCode.KEYCODE_K:
        case KeyCode.KEYCODE_S:
          keyAction = Action.ACT_DOWN;
          break;
        case KeyCode.KEYCODE_Z:
        case KeyCode.KEYCODE_U:
        case KeyCode.KEYCODE_Q:
        case KeyCode.KEYCODE_COMMA: //,
          keyAction = Action.ACT_DIG_LEFT;
          break;
        case KeyCode.KEYCODE_X:
        case KeyCode.KEYCODE_O:
        case KeyCode.KEYCODE_E:
        case KeyCode.KEYCODE_PERIOD: //.
          keyAction = Action.ACT_DIG_RIGHT;
          break;
        case KeyCode.KEYCODE_ESC: //help & pause
          if (State == GameState.GAME_PAUSE)
          {
            //gameResume();
          }
          else
          {
            //gamePause();
          }
          break;
        case KeyCode.KEYCODE_ENTER: //display hi-score
                                    //if (PlayMode == GameState.PLAY_CLASSIC)
                                    //{
                                    //  //menuIconDisable(1);
                                    //  //gamePause();
                                    //  //showScoreTable(playData, null, function() { menuIconEnable(); gameResume(); });
                                    //}
                                    //else
                                    //{
          keyAction = Action.ACT_UNKNOWN;
          //}
          break;
        default:
          keyAction = Action.ACT_UNKNOWN;
          //debug("keycode = " + code);	
          break;
      }
      //if (recordMode && code != KeyCode.KEYCODE_ESC) saveKeyCode(code, keyAction);
    }


    public void CheckGold()
    {
      if (goldCount <= 0) showHideLaddr();
    }

    public bool showHideLaddr()
    {
      var haveHLadder = false;
      for (var y = 0; y < Constants.NO_OF_TILES_Y; y++)
      {
        for (var x = 0; x < Constants.NO_OF_TILES_X; x++)
        {
          if (map[x][y].@base == TileType.HLADR_T)
          {
            haveHLadder = true;
            map[x][y].@base = TileType.LADDR_T;
            map[x][y].act = TileType.LADDR_T;
            map[x][y].setAlpha(1); //display laddr
          }
        }
      }
      goldComplete = true;
      _engine.Output.Enqueue<ShowHideLadderEvent>(Tick);
      return haveHLadder;
    }

    public void countTime(bool addTime)
    {

      if (curTime >= Constants.MAX_TIME_COUNT) return;
      if (addTime) curTime++;
      if (curTime > Constants.MAX_TIME_COUNT) { curTime = Constants.MAX_TIME_COUNT; }
    }

    public void ProcessRecordKey()
    {

    }

    public void processDigHole()
    {
      if ((int)_aiVersion < 3) return;
      if (++holeObj.curFrameIdx < holeObj.shapeFrame.Length)
      {
        Output.Enqueue<DigHoleProcessEvent>(Tick).Ratio = holeObj.curFrameIdx / (double)holeObj.shapeFrame.Length;

        holeObj.sprite.gotoAndStop(holeObj.shapeFrame[holeObj.curFrameIdx]);
        holeObj.sprite.currentAnimationFrame = holeObj.curFrameIdx;
      }
      else
      {
        Output.Enqueue<DigHoleProcessEvent>(Tick).Ratio = 1;

        digComplete();
      }
    }

    public void processFillHole()
    {
      for (var i = 0; i < fillHoleObj.Count;)
      {
        var curFillObj = fillHoleObj[i];
        var curIdx = curFillObj.curFrameIdx;

        if (++curFillObj.curFrameTime >= fillHoleTime[curIdx])
        {
          if (++curFillObj.curFrameIdx < fillHoleFrame.Length)
          {
            //change frame

            var evt = Output.Enqueue<FillHoleProcessEvent>(Tick);
            evt.Ratio = curFillObj.curFrameIdx / (double)fillHoleFrame.Length;
            evt.X = curFillObj.pos.x;
            evt.Y = curFillObj.pos.y;

            curFillObj.curFrameTime = 0;
            curFillObj.gotoAndStop(fillHoleFrame[curFillObj.curFrameIdx]);
          }
          else
          {
            var evt = Output.Enqueue<FillHoleProcessEvent>(Tick);
            evt.Ratio = 1;
            evt.X = curFillObj.pos.x;
            evt.Y = curFillObj.pos.y;

            //fill hole complete 
            fillComplete(curFillObj);
            continue;
          }
        }
        i++;
      }
    }

    public void stopDigging(int x, int y)
    {
      Output.Enqueue<StopDiggingEvent>(Tick);

      //(1) remove holeObj
      holeObj.sprite.removeAllEventListeners();
      holeObj.action = Action.ACT_STOP; //no digging
      holeObj.removeFromScene();

      //(2) fill hole
      y++;
      map[x][y].act = map[x][y].@base; //BLOCK_T
      Assert.IsTrue(map[x][y].@base == TileType.BLOCK_T, "fill hole != BLOCK_T");
      map[x][y].setAlpha(1); //display block

      //(3) change runner shape
      switch (runner.shape)
      {
        case Shape.digLeft:
          runner.sprite.gotoAndStop(Shape.runLeft);
          runner.shape = Shape.runLeft;
          runner.action = Action.ACT_STOP;
          break;
        case Shape.digRight:
          runner.sprite.gotoAndStop(Shape.runRight);
          runner.shape = Shape.runRight;
          runner.action = Action.ACT_STOP;
          break;
      }

      _engine.Sound.soundStop(Sounds.soundDig); //stop sound of digging
    }

    public bool isDigging()
    {
      var rc = false;

      if (holeObj.action == Action.ACT_DIGGING)
      {
        var x = holeObj.pos.x;
        var y = holeObj.pos.y;
        if (map[x][y].act == TileType.GUARD_T)
        { //guard come close to the digging hole !
          var id = guards.getGuardId(x, y);
          if (holeObj.sprite.currentAnimationFrame < holeObj.digLimit && guards[id].pos.yOffset > -Constants.H4)
          {
            stopDigging(x, y);
          }
          else
          {
            if ((int)_aiVersion >= 3)
            { //This is a bug while AI VERSION < 3
              map[x][y + 1].act = TileType.EMPTY_T; //assume hole complete
              rc = true;
            }
          }
        }
        else
        {
          switch (runner.shape)
          {
            case Shape.digLeft:
              if (holeObj.sprite.currentAnimationFrame > 2)
              {
                runner.sprite.gotoAndStop(Shape.runLeft); //change shape
                runner.shape = Shape.runLeft;
                runner.action = Action.ACT_STOP;
              }
              break;
            case Shape.digRight:
              if (holeObj.sprite.currentAnimationFrame > 2)
              {
                runner.sprite.gotoAndStop(Shape.runRight); //change shape
                runner.shape = Shape.runRight;
                runner.action = Action.ACT_STOP;
              }
              break;
          }
          rc = true;
        }
      }
      return rc;
    }

    public void digHole(Action action)
    {
      int x, y;
      Shape holeShape;

      if (action == Action.ACT_DIG_LEFT)
      {
        x = runner.pos.x - 1;
        y = runner.pos.y;

        runner.shape = Shape.digLeft;
        holeShape = Shape.digHoleLeft;

      }
      else
      { //DIG RIGHT

        x = runner.pos.x + 1;
        y = runner.pos.y;

        runner.shape = Shape.digRight;
        holeShape = Shape.digHoleRight;
      }

      _engine.Sound.soundPlay(Sounds.soundDig);
      map[x][y + 1].setAlpha(0); //hide block (replace with digging image)
      runner.sprite.gotoAndPlay(runner.shape);

      holeObj.action = Action.ACT_DIGGING;
      holeObj.pos = new Position { x = x, y = y };
      holeObj.sprite.setTransform(x, y);

      digTimeStart = recordCount;

      if ((int)_aiVersion < 3)
      {
        holeObj.sprite.gotoAndPlay(holeShape);
        holeObj.sprite.onAnimationEnded(digComplete);
      }
      else
      {
        if (action == Action.ACT_DIG_LEFT) holeShape = Shape.digHoleLeft;
        else holeShape = Shape.digHoleRight;

        holeObj.sprite.gotoAndStop(holeShape);
        holeObj.shapeFrame = holeShape == Shape.digHoleRight ? digHoleRight : digHoleLeft;
        holeObj.curFrameIdx = 0;
      }

      var evt = _engine.Output.Enqueue<StartDiggingEvent>(Tick);
      evt.X = x;
      evt.Y = y;

      holeObj.addToScene();
    }

    public void digComplete()
    {
      var x = holeObj.pos.x;
      var y = holeObj.pos.y + 1;

      _engine.Output.Enqueue<DiggingCompleteEvent>(Tick);

      map[x][y].act = TileType.EMPTY_T;
      holeObj.sprite.removeAllEventListeners();
      holeObj.action = Action.ACT_STOP; //no digging
      holeObj.removeFromScene();

      fillHole(x, y);
    }

    public void fillHole(int x, int y)
    {
      var fillSprite = new Sprite(_config.createHole());
      _sprites.Add(fillSprite);
      fillSprite.gotoAndPlay(Shape.fillHole);
      fillSprite.pos.x = x;
      fillSprite.pos.y = y;

      if ((int)_aiVersion < 3)
      {
        fillSprite.onAnimationEnded(() => fillComplete(fillSprite));
        fillSprite.play();
      }
      else
      {
        fillSprite.curFrameIdx = 0;
        fillSprite.curFrameTime = -1;
        fillSprite.gotoAndStop(fillHoleFrame[0]);
      }

      var evt = _engine.Output.Enqueue<StartFillHoleEvent>(Tick);
      evt.X = x;
      evt.Y = y;

      fillHoleObj.Add(fillSprite);

      fillHoleTimeStart = recordCount; //for debug
    }

    public void fillComplete(Sprite fillObj)
    {
      //don't use "divide command", it will cause loss of accuracy while scale changed (ex: tileScale = 0.6...)
      //var x = this.x / tileWScale | 0; //this : scope default to the dispatcher
      //var y = this.y / tileHScale | 0;

      var x = fillObj.pos.x;
      var y = fillObj.pos.y; //get position 

      var evt = _engine.Output.Enqueue<EndFillHoleEvent>(Tick);
      evt.X = x;
      evt.Y = y;

      map[x][y].setAlpha(1); //display block
      fillObj.removeAllEventListeners();
      fillObj.removeFromScene();
      _sprites.Remove(fillObj);
      removeFillHoleObj(fillObj);

      switch (map[x][y].act)
      {
        case TileType.RUNNER_T: // runner dead
          //loadingTxt.text = "RUNNER DEAD"; 
          State = GameState.GAME_RUNNER_DEAD;
          runner.hideRunner();
          runner.sprite.setAlpha(0); //hidden runner --> dead
          break;
        case TileType.GUARD_T: //guard dead
          var id = guards.getGuardId(x, y);
          if ((int)_aiVersion >= 3 && guards[id].action == Action.ACT_IN_HOLE) guards.removeFromShake(id);
          if (guards[id].hasGold > 0)
          { //guard has gold and not fall into the hole
            decGold();
            guards[id].hasGold = 0;
            guards.guardRemoveRedhat(guards[id]); //9/4/2016	
          }
          guards.guardReborn(x, y);
          if (PlayMode == PlayMode.PLAY_CLASSIC || PlayMode == PlayMode.PLAY_AUTO || PlayMode == PlayMode.PLAY_DEMO)
          {
            drawScore(Constants.SCORE_GUARD_DEAD);
          }
          else
          {
            //for modern mode & edit mode
            drawGuard(1); //guard dead, add count
          }
          break;
      }
      map[x][y].act = TileType.BLOCK_T;
    }

    public void removeFillHoleObj(Sprite spriteObj)
    {
      for (var i = 0; i < fillHoleObj.Count; i++)
      {
        if (fillHoleObj[i] == spriteObj)
        {
          fillHoleObj.RemoveAt(i);
          return;
        }
      }
      Assert.IsTrue(false, "design error !");
    }

    public void setRunnerDead()
    {
      if (!godMode)
      {
        Output.Enqueue<RunnerDeadEvent>(Tick);

        State = GameState.GAME_RUNNER_DEAD;
      }
    }

    public void removeGold(int x, int y)
    {
      map[x][y].@base = TileType.EMPTY_T;
      removeFromScene(x, y);
      //map[x][y].bitmap = null;

      var evt = _engine.Output.Enqueue<RemoveGoldEvent>(Tick);
      evt.X = x;
      evt.Y = y;
    }

    public void addGold(int x, int y)
    {
      map[x][y].@base = TileType.GOLD_T;
      //map[x][y].bitmap.name = "gold";
      addToScene(x, y);

      var evt = _engine.Output.Enqueue<AddGoldEvent>(Tick);
      evt.X = x;
      evt.Y = y;
    }

    private void addToScene(int x, int y)
    {

    }

    private void removeFromScene(int x, int y)
    {

    }

    public void decGold()
    {
      if (--goldCount <= 0)
      {
        showHideLaddr();
        if (runner.pos.y > 0)
        {
          Sound.soundPlay(Sounds.goldFinish);
          //if (curTheme == "C64") soundPlay("goldFinish" + ((curLevel - 1) % 6 + 1)); //six sounds
          //else soundPlay("goldFinish"); //for all apple2 mode, 9/12/2015
        }
      }
    }

    public void drawScore(int scoreGetGold)
    {

    }

    public void drawGold(int count)
    {

    }

    private void drawGuard(int count)
    {

    }

    public void drawTime(int count)
    {

    }

    public Point getDemoBornPos()
    {
      return new Point();
    }

    public void saveRecordBornPos(int x, int bornY)
    {

    }

    public Point getRecordBornPos()
    {
      return new Point();
    }

    public double random()
    {
      return _random.NextDouble();
    }

    public int getDemoGold(Guard curGuard)
    {
      return 0;
    }

    public void processRecordGold(Guard curGuard)
    {

    }

    public void updateSprites(int ticks)
    {
      for (int i = 0; i < ticks; i++)
      {
        foreach (var sprite in _sprites)
        {
          sprite.tick(1);
        }
      }
    }
  }
}
