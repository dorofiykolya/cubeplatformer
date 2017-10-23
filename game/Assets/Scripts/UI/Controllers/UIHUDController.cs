using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.UI.Components;
using Game.UI.HUDs;
using Game.UI.Providers;
using Game.UI.Windows;
using Injection;
using Utils;
using Utils.Collections;

namespace Game.UI.Controllers
{
  public class UIHUDReference
  {
    private readonly Lifetime.Definition _definition;

    public UIHUDReference(Lifetime.Definition definition)
    {
      _definition = definition;
    }

    public void Close()
    {
      _definition.Terminate();
    }
  }

  public class UIHUDController : UIController
  {
    [Inject]
    private IInjector _injector;
    [Inject]
    private UISceneController _sceneController;

    private readonly Dictionary<Type, string> _map = new Dictionary<Type, string>();
    private readonly List<UIHUD> _opened = new List<UIHUD>();
    private readonly LinkedQueue<Action<Action>> _queue = new LinkedQueue<Action<Action>>();
    private bool _inOpenProcess;

    public UIHUDReference Open<T>(Action<UIHUD> onOpen = null) where T : UIHUD
    {
      var definition = Lifetime.Define(Lifetime);
      var shell = new UIHUDReference(definition);
      Enqueue(typeof(T), onOpen, definition);
      return shell;
    }

    protected override void OnInitialize()
    {
      foreach (var map in new UIHUDProvider().GetMap())
      {
        _map[map.Type] = map.Path;
        _injector.Map(map.Type).ToFactory(map.Type);
      }
    }

    private void Enqueue(Type type, Action<UIHUD> onOpen, Lifetime.Definition lifetimeDefinition)
    {
      var intersectLifetime = Lifetime.Intersection(lifetimeDefinition.Lifetime, Lifetime);
      Action<Action> action = (callback) =>
      {
        var path = _map[type];
        Context.ResourceManager.GetPrefab(path).LoadAsync(intersectLifetime.Lifetime, result =>
        {
          var windowMediator = (UIHUD)_injector.Get(type);
          var windowComponent = result.Instantiate<UIHUDComponent>();
          GameObject.DontDestroyOnLoad(windowComponent.gameObject);
          _injector.Inject(windowMediator);
          _sceneController.SceneComponent.HUDRoot.AddChild(windowComponent.transform);
          windowComponent.gameObject.AddComponent<SignalMonoBehaviour>().DestroySignal.Subscribe(intersectLifetime.Lifetime, intersectLifetime.Terminate);
          MethodInvoker<UIHUD, InitializeAttribute>.Invoke(windowMediator, intersectLifetime, windowComponent);
          _opened.Add(windowMediator);
          MethodInvoker<UIHUD, UIHUDOpenAttribute>.Invoke(windowMediator);
          if (onOpen != null) onOpen(windowMediator);
          callback();
          intersectLifetime.Lifetime.AddAction(() =>
          {
            MethodInvoker<UIHUD, UIHUDCloseAttribute>.Invoke(windowMediator);
            _opened.Remove(windowMediator);
            result.Release(windowComponent);
            result.Collect();
          });
        });
      };

      _queue.Enqueue(action);
      intersectLifetime.Lifetime.AddAction(() =>
      {
        _queue.Remove(action);
        if (_queue.Count == 0)
        {
          _inOpenProcess = false;
        }
      });

      if (!_inOpenProcess)
      {
        OpenProcess();
      }
    }

    private void OpenProcess()
    {
      if (_sceneController.SceneComponent == null)
      {
        _inOpenProcess = true;
        _sceneController.SubscribeOnSceneReady(Lifetime, Execute);
      }
      else
      {
        Execute();
      }
    }

    private void Execute()
    {
      if (_queue.Count != 0)
      {
        _queue.Dequeue()(() =>
        {
          if (_queue.Count != 0)
          {
            OpenProcess();
          }
          else
          {
            _inOpenProcess = false;
          }
        });
      }
    }
  }
}