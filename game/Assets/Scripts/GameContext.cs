using Game;
using Game.Commands;
using Game.Controllers;
using Game.Inputs;
using Game.Messages;
using Game.Messages.Commands;
using Game.Modules;
using Game.Providers;
using Game.UI;
using Injection;
using System.Collections;
using UnityEngine;
using Utils;
using Utils.Threading;

namespace Game
{
  public class GameContext : IContext
  {
    public static GameContext Context { get; private set; }

    private readonly Lifetime _lifetime;
    private readonly TimeManager _timeManager;
    private readonly IDispatcher _dispatcher;
    private readonly PrefabResourceManager _resourceManager;
    private readonly GameControllers _controllers;
    private readonly Preloader _preloader;
    private readonly GameProviders _providers;
    private readonly Transform _rootTransform;
    private readonly UIContext _uiContext;
    private readonly InputContext _inputContext;
    private readonly ILogger _logger;
    private readonly Injector _injector;
    private readonly CommandMap _commandMap;
    private GameNavigator _navigator;

    public GameContext(Lifetime lifetime, GameStartBehaviour behaviour)
    {
      Context = this;

      _lifetime = lifetime;
      _rootTransform = behaviour.transform;
      _logger = Debug.unityLogger;

      _injector = new Injector();

      _commandMap = new CommandMap(this);

      _injector.Map<CommandMap>().ToValue(_commandMap);
      _injector.Map<GameStartBehaviour>().ToValue(behaviour);

      _providers = new GameProviders(lifetime, behaviour);
      _timeManager = new TimeManager(lifetime, behaviour);
      _resourceManager = new PrefabResourceManager(_providers.CoroutineProvider);
      _dispatcher = behaviour.gameObject.GetComponent<UnityDispatcher>() ?? behaviour.gameObject.AddComponent<UnityDispatcher>();
      _preloader = new Preloader(_lifetime);
      _inputContext = new GameInputContext(this);

      _uiContext = new UIContext(this, _injector);
      _controllers = new GameControllers(lifetime, this, _injector, new GameControllersProvider());
      Initializer<UI.UIContext>.Initialize(_uiContext);


      _lifetime.AddAction(() =>
        {
          _injector.Dispose();
          _resourceManager.Dispose();
        }
      );

      _navigator = new GameNavigator(this);

      CommandMap.Map<StartMessage>().RegisterCommand(lt => new StartCommand());

      Tell(new StartMessage());
    }

    public InputContext InputContext { get { return _inputContext.Current; } }
    public UIContext UIContext { get { return _uiContext; } }
    public Transform RootTransform { get { return _rootTransform; } }
    public GameProviders Providers { get { return _providers; } }
    public Preloader Preloader { get { return _preloader; } }
    public Lifetime Lifetime { get { return _lifetime; } }
    public ILogger Logger { get { return _logger; } }
    public TimeManager Time { get { return _timeManager; } }
    public ICoroutineProvider CoroutineProvider { get { return _providers.CoroutineProvider; } }
    public IDispatcher Dispatcher { get { return _dispatcher; } }
    public PrefabResourceManager ResourceManager { get { return _resourceManager; } }
    public GameControllers Controllers { get { return _controllers; } }
    public IInjector Injector { get { return _injector; } }
    public CommandMap CommandMap { get { return _commandMap; } }
    public GameNavigator Navigator { get { return _navigator; } }

    public void Tell(object message)
    {
      CommandMap.Tell(message);
    }

    public void StartCoroutine(Lifetime lifetime, IEnumerator enumerator)
    {
      var coroutine = _providers.CoroutineProvider.StartCoroutine(enumerator);
      if (coroutine != null)
      {
        Lifetime.Intersection(lifetime, _lifetime).Lifetime.AddAction(() =>
        {
          _providers.CoroutineProvider.StopCoroutine(coroutine);
        });
      }
    }
  }
}