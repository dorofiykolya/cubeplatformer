using Game.Components;
using Injection;

namespace Game.Controllers
{
  public class GameSoundController : GameController
  {
    [Inject]
    private GameStartBehaviour _behaviour;

    private AudioPlayerComponent _player;

    protected override void OnInitialize()
    {
      _player = _behaviour.GetComponent<AudioPlayerComponent>();
    }

    public void Play(string sound)
    {
      _player.Play(sound);
    }

    public void Stop(string sound)
    {
      _player.Stop(sound);
    }

    public void PlayLoop(string sound)
    {
      _player.PlayLoop(sound);
    }
  }
}
