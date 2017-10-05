using System;
using ClassicLogic.Utils;

namespace ClassicLogic.Engine
{
  public class Engine
  {
    private readonly EngineState _state;
    private readonly EngineOutput _output;
    private readonly EngineSound _sound;
    private readonly AIVersion _aiVersion;
    private readonly Mode _mode;

    public Engine(AIVersion aiVersion, LevelReader level, Mode mode = Mode.Classic)
    {
      _aiVersion = aiVersion;
      _mode = mode;

      _output = new EngineOutput();
      _sound = new EngineSound();

      var config = Constants.CONFIGURATION[aiVersion];

      _state = new EngineState(aiVersion, mode, config, LevelParser.Parse(level, config.maxGuard), this);
    }

    public Mode Mode { get { return _mode; } }
    public AIVersion AiVersion { get { return _aiVersion; } }
    public EngineState State { get { return _state; } }
    public EngineSound Sound { get { return _sound; } }
    public EngineOutput Output { get { return _output; } }

    public void SetAction(KeyCode code)
    {
      State.pressKey(code);
    }

    public void FastForward(int tick)
    {
      if (tick <= State.Tick) throw new ArgumentException();

      var lastTick = State.Tick;
      var delta = tick - lastTick;
      State.Tick = tick;
      switch (State.State)
      {
        case GameState.GAME_START:
          if (State.keyAction != Action.ACT_STOP && State.keyAction != Action.ACT_UNKNOWN)
          {
            State.State = GameState.GAME_RUNNING;
            State.playTickTimer = 0;
            State.CheckGold();
          }
          break;
        case GameState.GAME_RUNNING:
          Play(delta, tick);
          break;
      }
    }

    private void Play(int delta, int tick)
    {
      if (State.goldComplete && State.runner.pos.x == 0 && State.runner.pos.y == 0)
      {
        State.State = GameState.GAME_FINISH;
        return;
      }

      if (++State.playTickTimer >= Constants.TICK_COUNT_PER_TIME)
      {
        if (State.PlayMode != PlayMode.PLAY_CLASSIC && State.PlayMode != PlayMode.PLAY_AUTO && State.PlayMode != PlayMode.PLAY_DEMO) State.drawTime(1);
        else State.countTime(true);
        State.playTickTimer = 0;
      }

      State.updateSprites(delta);

      if (State.PlayMode == PlayMode.PLAY_AUTO || State.PlayMode == PlayMode.PLAY_DEMO || State.PlayMode == PlayMode.PLAY_DEMO_ONCE) PlayDemo();
      if (State.recordMode == RecordMode.RECORD_KEY) State.ProcessRecordKey();
      if (!State.isDigging()) State.runner.moveRunner();
      else State.processDigHole();
      if (State.State != GameState.GAME_RUNNER_DEAD) State.guards.moveGuard();

      if ((int)_aiVersion >= 3)
      {
        State.guards.processGuardShake();
        State.processFillHole();
        State.guards.processReborn();
      }
    }

    private void PlayDemo()
    {

    }
  }
}
