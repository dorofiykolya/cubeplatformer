using ClassicLogic.Outputs;

namespace ClassicLogic.Engine
{
  public class EngineSound
  {
    private readonly Engine _engine;

    public EngineSound(Engine engine)
    {
      _engine = engine;
    }

    public void SoundStop(Sounds name)
    {
      _engine.Output.Enqueue<StopSoundEvent>(_engine.State.Tick).Sound = name;
    }

    public void SoundPlay(Sounds name)
    {
      _engine.Output.Enqueue<PlaySoundEvent>(_engine.State.Tick).Set(name, false);
    }

    public void LoopSoundPlay(Sounds name)
    {
      _engine.Output.Enqueue<PlaySoundEvent>(_engine.State.Tick).Set(name, true);
    }
  }
}
