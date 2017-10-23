using System;
using ClassicLogic.Utils;

namespace ClassicLogic.Engine
{
  public class Engine
  {
    private readonly EngineState _state;
    private readonly EngineOutput _output;
    private readonly EngineSound _sound;

    public Engine(LevelReader level, double speedScale = 1.0)
    {
      _output = new EngineOutput();
      _sound = new EngineSound(this);

      var config = Constants.Configuration[AiVersion.V4];

      _state = new EngineState(config, LevelParser.Parse(level, config.MaxGuard), this, speedScale);
    }

    public EngineState State { get { return _state; } }
    public EngineSound Sound { get { return _sound; } }
    public EngineOutput Output { get { return _output; } }

    public void SetAction(InputAction code)
    {
      State.PressAction(code);
    }

    public void FastForward(int tick)
    {
      if (tick <= State.Tick) throw new ArgumentException();

      var lastTick = State.Tick;
      var delta = tick - lastTick;
      State.Tick = tick;
      switch (State.State)
      {
        case GameState.GameStart:
          if (State.KeyAction != Action.Stop && State.KeyAction != Action.Unknown)
          {
            State.State = GameState.GameRunning;
            State.PlayTickTimer = 0;
            State.CheckGold();
          }
          break;
        case GameState.GameRunning:
          Play(delta, tick);
          break;
      }
    }

    private void Play(int delta, int tick)
    {
      if (State.GoldComplete && State.Runner.Position.X == 0 && State.Runner.Position.Y == 0)
      {
        State.State = GameState.GameFinish;
        return;
      }

      if (++State.PlayTickTimer >= Constants.TickCountPerTime)
      {
        State.DrawTime(1);

        State.PlayTickTimer = 0;
      }
      
      if (!State.IsDigging()) State.Runner.Move();
      else State.ProcessDigHole();
      if (State.State != GameState.GameRunnerDead) State.Guards.Move();

      State.Guards.ProcessGuardShake();
      State.ProcessFillHole();
      State.Guards.ProcessReborn();
    }
  }
}
