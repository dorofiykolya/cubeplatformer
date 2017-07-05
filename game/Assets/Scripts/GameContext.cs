﻿using System.Collections;
using Game.Managers;
using Game.Providers;
using Game.Views;
using Injection;
using UnityEngine;
using Utils;
using Utils.Threading;

namespace Game
{
  public class GameContext
  {
    private readonly Lifetime _lifetime;
    private readonly TimeManager _timeManager;
    private readonly IDispatcher _dispatcher;
    private readonly ResourceManager _resourceManager;
    private readonly GameStateManager _stateManager;
    private readonly GameManagers _managers;
    private readonly Preloader _preloader;
    private readonly GameProviders _providers;
    private readonly Transform _rootTransform;
    private readonly ViewContext _viewContext;

    public GameContext(Lifetime lifetime, GameStartBehaviour behaviour)
    {
      _lifetime = lifetime;
      _rootTransform = behaviour.transform;

      var injector = new Injector();

      _providers = new GameProviders(lifetime, behaviour);
      _timeManager = new TimeManager(lifetime, behaviour);
      _resourceManager = new ResourceManager(_providers.CoroutineProvider);
      _stateManager = new GameStateManager(lifetime);
      _dispatcher = behaviour.gameObject.GetComponent<UnityDispatcher>() ?? behaviour.gameObject.AddComponent<UnityDispatcher>();
      _preloader = new Preloader(_lifetime);
      _managers = new GameManagers(lifetime, this, injector, new GameManagersProvider());

      _viewContext = new ViewContext(this, injector);

      _lifetime.AddAction(() => injector.Dispose());
    }

    public ViewContext ViewContext { get { return _viewContext; } }
    public Transform RootTransform { get { return _rootTransform; } }
    public GameProviders Providers { get { return _providers; } }
    public Preloader Preloader { get { return _preloader; } }
    public Lifetime Lifetime { get { return _lifetime; } }
    public TimeManager Time { get { return _timeManager; } }
    public ICoroutineProvider CoroutineProvider { get { return _providers.CoroutineProvider; } }
    public IDispatcher Dispatcher { get { return _dispatcher; } }
    public ResourceManager ResourceManager { get { return _resourceManager; } }
    public GameManagers Managers { get { return _managers; } }

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