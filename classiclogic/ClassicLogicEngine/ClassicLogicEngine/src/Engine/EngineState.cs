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
    private readonly Configuration _config;
    private readonly Random _random;

    public readonly double XMove;
    public readonly double YMove;

    public int PlayTickTimer;
    public Action KeyAction;
    public int GoldCount;
    public Runner Runner;
    public bool GoldComplete;
    public int CurTime;
    public EngineGuards Guards;
    public HoleObj HoleObj;
    public Tile[][] Map;
    public RandomRange BornX;

    public List<Sprite> FillHoleObj = new List<Sprite>();

    private readonly int[] _digHoleLeft = { 0, 1, 2, 2, 3, 4, 4, 5, 6, 6, 7 };
    private readonly int[] _digHoleRight = { 8, 9, 10, 10, 11, 12, 12, 13, 14, 14, 15 };
    private readonly int[] _fillHoleFrame = { 16, 17, 18, 19 };
    private readonly int[] _fillHoleTime = { 166, 8, 8, 4 };
    public int[] ShakeTime;
    public int[][] MovePolicy;

    public EngineState(Configuration config, LevelMap levelMap, Engine engine)
    {
      _config = config;
      _levelMap = levelMap;
      _engine = engine;

      _random = new Random();

      XMove = config.XMoveBase;
      YMove = config.YMoveBase;

      Map = _levelMap.ToArray();
      GoldCount = levelMap.GoldCount;

      MovePolicy = config.MovePolicy;
      ShakeTime = config.ShakeTime;
      BornX = new RandomRange(0, _levelMap.XCount, 0);
      Guards = new EngineGuards(Map, BornX, this);
      Guards.MaxGuard = _levelMap.MaxGuard;

      HoleObj = new HoleObj(Map, this);
      Runner = new Runner(Map, Guards, this);

      for (int x = 0; x < _levelMap.XCount; x++)
      {
        for (int y = 0; y < _levelMap.YCount; y++)
        {
          var tile = _levelMap[x][y];
          if (tile.Act == TileType.GUARD_T)
          {
            var curGuard = new Guard(x, y, Guards.GuardCount);
            Guards.Guard.Add(curGuard);
          }
          else if (tile.Act == TileType.RUNNER_T)
          {
            Runner.Position.X = x;
            Runner.Position.Y = y;
          }
        }
      }

      var evt = engine.Output.Enqueue<InitializeEvent>(0);
      evt.Map = Map.Select(i => i.Select(a => a.Base).ToArray()).ToArray();
      evt.Guard = Guards.Guard.Select(g => new InitializeEvent.GuardData
      {
        Position = new Point
        {
          x = g.Position.X,
          y = g.Position.Y
        },
        Id = g.Id,
        Shape = g.Shape,
        Action = g.Action
      }).ToArray();
      evt.Runner = new Point
      {
        x = Runner.Position.X,
        y = Runner.Position.Y
      };
      evt.RunnerAction = Runner.Action;
      evt.RunnerShape = Runner.Shape;
    }

    public int MaxTileX { get { return _levelMap.XCount - 1; } }
    public int MaxTileY { get { return _levelMap.YCount - 1; } }
    public GameState State { get; set; }
    public int Tick { get; set; }
    public EngineSound Sound { get { return _engine.Sound; } }
    public EngineOutput Output { get { return _engine.Output; } }

    public void PressAction(InputAction action)
    {
      switch (action)
      {
        case InputAction.Up:
          KeyAction = Action.Up;
          break;
        case InputAction.Down:
          KeyAction = Action.Down;
          break;
        case InputAction.Left:
          KeyAction = Action.Left;
          break;
        case InputAction.Right:
          KeyAction = Action.Right;
          break;
        case InputAction.DigLeft:
          KeyAction = Action.DigLeft;
          break;
        case InputAction.DigRight:
          KeyAction = Action.DigRight;
          break;
        case InputAction.Unknown:
          KeyAction = Action.Unknown;
          break;
        default:
          KeyAction = Action.Unknown;
          break;
      }
    }

    public void CheckGold()
    {
      if (GoldCount <= 0) ShowHideLadder();
    }

    public bool ShowHideLadder()
    {
      var haveHLadder = false;
      var width = _levelMap.XCount;
      var height = _levelMap.YCount;
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          if (Map[x][y].Base == TileType.HLADR_T)
          {
            haveHLadder = true;
            Map[x][y].Base = TileType.LADDR_T;
            Map[x][y].Act = TileType.LADDR_T;
          }
        }
      }
      GoldComplete = true;
      _engine.Output.Enqueue<ShowHideLadderEvent>(Tick);
      return haveHLadder;
    }

    public void ProcessDigHole()
    {
      if (++HoleObj.CurFrameIdx < HoleObj.ShapeFrame.Length)
      {
        Output.Enqueue<DigHoleProcessEvent>(Tick).Ratio = HoleObj.CurFrameIdx / (double)HoleObj.ShapeFrame.Length;

        HoleObj.Sprite.GotoAndStop(HoleObj.ShapeFrame[HoleObj.CurFrameIdx]);
        HoleObj.Sprite.CurrentFrame = HoleObj.CurFrameIdx;
      }
      else
      {
        Output.Enqueue<DigHoleProcessEvent>(Tick).Ratio = 1;

        DigComplete();
      }
    }

    public void ProcessFillHole()
    {
      for (var i = 0; i < FillHoleObj.Count;)
      {
        var curFillObj = FillHoleObj[i];
        var curIdx = curFillObj.CurrentFrameId;

        if (++curFillObj.CurrentFrameTime >= _fillHoleTime[curIdx])
        {
          if (++curFillObj.CurrentFrameId < _fillHoleFrame.Length)
          {
            //change frame

            var evt = Output.Enqueue<FillHoleProcessEvent>(Tick);
            evt.Ratio = curFillObj.CurrentFrameId / (double)_fillHoleFrame.Length;
            evt.X = curFillObj.Position.X;
            evt.Y = curFillObj.Position.Y;

            curFillObj.CurrentFrameTime = 0;
            curFillObj.GotoAndStop(_fillHoleFrame[curFillObj.CurrentFrameId]);
          }
          else
          {
            var evt = Output.Enqueue<FillHoleProcessEvent>(Tick);
            evt.Ratio = 1;
            evt.X = curFillObj.Position.X;
            evt.Y = curFillObj.Position.Y;

            //fill hole complete 
            FillComplete(curFillObj);
            continue;
          }
        }
        i++;
      }
    }

    public void StopDigging(int x, int y)
    {
      Output.Enqueue<StopDiggingEvent>(Tick);

      //(1) remove holeObj
      HoleObj.Action = Action.Stop; //no digging
      HoleObj.RemoveFromScene();

      //(2) fill hole
      y++;
      Map[x][y].Act = Map[x][y].Base; //BLOCK_T
      Assert.IsTrue(Map[x][y].Base == TileType.BLOCK_T, "fill hole != BLOCK_T");

      //(3) change runner shape
      switch (Runner.Shape)
      {
        case Shape.DigLeft:
          Runner.Sprite.GotoAndStop(Shape.RunLeft);
          Runner.Shape = Shape.RunLeft;
          Output.Enqueue<RunnerShapeEvent>(Tick).Shape = Runner.Shape;
          Runner.Action = Action.Stop;
          Output.Enqueue<RunnerActionEvent>(Tick).Action = Action.Stop;
          break;
        case Shape.DigRight:
          Runner.Sprite.GotoAndStop(Shape.RunRight);
          Runner.Shape = Shape.RunRight;
          Output.Enqueue<RunnerShapeEvent>(Tick).Shape = Runner.Shape;
          Runner.Action = Action.Stop;
          Output.Enqueue<RunnerActionEvent>(Tick).Action = Action.Stop;
          break;
      }

      _engine.Sound.SoundStop(Sounds.Dig); //stop sound of digging
    }

    public bool IsDigging()
    {
      var rc = false;

      if (HoleObj.Action == Action.Digging)
      {
        var x = HoleObj.Position.X;
        var y = HoleObj.Position.Y;
        if (Map[x][y].Act == TileType.GUARD_T)
        { //guard come close to the digging hole !
          var id = Guards.GetGuardId(x, y);
          if (HoleObj.Sprite.CurrentFrame < HoleObj.DigLimit && Guards[id].Position.YOffset > -Constants.H4)
          {
            StopDigging(x, y);
          }
          else
          {
            //This is a bug while AI VERSION < 3
            Map[x][y + 1].Act = TileType.EMPTY_T; //assume hole complete
            rc = true;
          }
        }
        else
        {
          switch (Runner.Shape)
          {
            case Shape.DigLeft:
              if (HoleObj.Sprite.CurrentFrame > 2)
              {
                Runner.Sprite.GotoAndStop(Shape.RunLeft); //change shape
                Runner.Shape = Shape.RunLeft;

                Output.Enqueue<RunnerShapeEvent>(Tick).Shape = Runner.Shape;

                Runner.Action = Action.Stop;

                Output.Enqueue<RunnerActionEvent>(Tick).Action = Action.Stop;
              }
              break;
            case Shape.DigRight:
              if (HoleObj.Sprite.CurrentFrame > 2)
              {
                Runner.Sprite.GotoAndStop(Shape.RunRight); //change shape
                Runner.Shape = Shape.RunRight;

                Output.Enqueue<RunnerShapeEvent>(Tick).Shape = Runner.Shape;

                Runner.Action = Action.Stop;

                Output.Enqueue<RunnerActionEvent>(Tick).Action = Action.Stop;
              }
              break;
          }
          rc = true;
        }
      }
      return rc;
    }

    public void DigHole(Action action)
    {
      int x, y;
      Shape holeShape;

      if (action == Action.DigLeft)
      {
        x = Runner.Position.X - 1;
        y = Runner.Position.Y;

        Runner.Shape = Shape.DigLeft;
      }
      else
      { //DIG RIGHT

        x = Runner.Position.X + 1;
        y = Runner.Position.Y;

        Runner.Shape = Shape.DigRight;
      }

      _engine.Sound.SoundPlay(Sounds.Dig);

      Output.Enqueue<RunnerShapeEvent>(Tick).Shape = Runner.Shape;

      Runner.Sprite.GotoAndPlay(Runner.Shape);

      HoleObj.Action = Action.Digging;
      HoleObj.Position = new Position { X = x, Y = y };

      if (action == Action.DigLeft) holeShape = Shape.DigHoleLeft;
      else holeShape = Shape.DigHoleRight;

      HoleObj.Sprite.GotoAndStop(holeShape);
      HoleObj.ShapeFrame = holeShape == Shape.DigHoleRight ? _digHoleRight : _digHoleLeft;
      HoleObj.CurFrameIdx = 0;

      var evt = _engine.Output.Enqueue<StartDiggingEvent>(Tick);
      evt.X = x;
      evt.Y = y;

      HoleObj.AddToScene();
    }

    public void DigComplete()
    {
      var x = HoleObj.Position.X;
      var y = HoleObj.Position.Y + 1;

      _engine.Output.Enqueue<DiggingCompleteEvent>(Tick);

      Map[x][y].Act = TileType.EMPTY_T;
      HoleObj.Action = Action.Stop; //no digging
      HoleObj.RemoveFromScene();

      FillHole(x, y);
    }

    public void FillHole(int x, int y)
    {
      var fillSprite = new Sprite();
      fillSprite.GotoAndPlay(Shape.FillHole);
      fillSprite.Position.X = x;
      fillSprite.Position.Y = y;

      fillSprite.CurrentFrameId = 0;
      fillSprite.CurrentFrameTime = -1;
      fillSprite.GotoAndStop(_fillHoleFrame[0]);

      var evt = _engine.Output.Enqueue<StartFillHoleEvent>(Tick);
      evt.X = x;
      evt.Y = y;

      FillHoleObj.Add(fillSprite);
    }

    public void FillComplete(Sprite fillObj)
    {
      //don't use "divide command", it will cause loss of accuracy while scale changed (ex: tileScale = 0.6...)
      //var x = this.x / tileWScale | 0; //this : scope default to the dispatcher
      //var y = this.y / tileHScale | 0;

      var x = fillObj.Position.X;
      var y = fillObj.Position.Y; //get position 

      var evt = _engine.Output.Enqueue<EndFillHoleEvent>(Tick);
      evt.X = x;
      evt.Y = y;

      RemoveFillHoleObj(fillObj);

      switch (Map[x][y].Act)
      {
        case TileType.RUNNER_T: // runner dead
          //loadingTxt.text = "RUNNER DEAD"; 
          SetRunnerDead();
          break;
        case TileType.GUARD_T: //guard dead
          var id = Guards.GetGuardId(x, y);
          if (Guards[id].Action == Action.InHole) Guards.RemoveFromShake(id);
          if (Guards[id].HasGold > 0)
          { //guard has gold and not fall into the hole
            DecGold();
            Guards[id].HasGold = 0;
            Guards.GuardRemoveRedhat(Guards[id]); //9/4/2016	
          }
          Guards.GuardReborn(x, y);
          DrawScore(Constants.ScoreGuardDead);
          //for modern mode & edit mode
          DrawGuard(1); //guard dead, add count
          break;
      }
      Map[x][y].Act = TileType.BLOCK_T;
    }

    public void RemoveFillHoleObj(Sprite spriteObj)
    {
      for (var i = 0; i < FillHoleObj.Count; i++)
      {
        if (FillHoleObj[i] == spriteObj)
        {
          FillHoleObj.RemoveAt(i);
          return;
        }
      }
      Assert.IsTrue(false, "design error !");
    }

    public void SetRunnerDead()
    {
      Output.Enqueue<RunnerDeadEvent>(Tick);

      State = GameState.GameRunnerDead;
    }

    public void RemoveGold(int x, int y)
    {
      Map[x][y].Base = TileType.EMPTY_T;

      var evt = _engine.Output.Enqueue<RemoveGoldEvent>(Tick);
      evt.X = x;
      evt.Y = y;
    }

    public void AddGold(int x, int y)
    {
      Map[x][y].Base = TileType.GOLD_T;

      var evt = _engine.Output.Enqueue<AddGoldEvent>(Tick);
      evt.X = x;
      evt.Y = y;
    }

    public void DecGold()
    {
      if (--GoldCount <= 0)
      {
        ShowHideLadder();
        if (Runner.Position.Y > 0)
        {
          Sound.SoundPlay(Sounds.GoldFinish);
        }
      }
    }

    public void DrawScore(int scoreGetGold)
    {

    }

    public void DrawGold(int count)
    {

    }

    private void DrawGuard(int count)
    {

    }

    public void DrawTime(int count)
    {

    }

    public double Random()
    {
      return _random.NextDouble();
    }
  }
}
