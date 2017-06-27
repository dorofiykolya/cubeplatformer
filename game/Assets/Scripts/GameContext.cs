using System.Collections;
using Utils;
using Utils.Threading;

namespace Game
{
  public class GameContext
  {
    private readonly Lifetime _lifetime;
    private readonly TimeManager _timeManager;
    private readonly ICoroutineProvider _coroutineProvider;
    private readonly IDispatcher _dispatcher;
    private readonly ResourceManager _resourceManager;
    private readonly GameStateManager _stateManager;

    public GameContext(Lifetime lifetime, GameStartBehaviour behaviour)
    {
      _lifetime = lifetime;
      _timeManager = new TimeManager(behaviour);
      _coroutineProvider = new CoroutineProvider(behaviour);
      _resourceManager = new ResourceManager(_coroutineProvider);
      _stateManager = new GameStateManager(lifetime);
      _dispatcher = behaviour.gameObject.AddComponent<UnityDispatcher>();
    }

    public TimeManager Time { get { return _timeManager; } }
    public ICoroutineProvider CoroutineProvider { get { return _coroutineProvider; } }
    public IDispatcher Dispatcher { get { return _dispatcher; } }
    public ResourceManager ResourceManager { get { return _resourceManager; } }

    public void StartCoroutine(Lifetime lifetime, IEnumerator enumerator)
    {
      var coroutine = _coroutineProvider.StartCoroutine(enumerator);
      if (coroutine != null)
      {
        Lifetime.Intersection(lifetime, _lifetime).Lifetime.AddAction(() =>
        {
          _coroutineProvider.StopCoroutine(coroutine);
        });
      }
    }
  }
}