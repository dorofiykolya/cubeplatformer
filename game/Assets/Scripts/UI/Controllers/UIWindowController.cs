using System;
using UnityEngine;
using System.Collections.Generic;
using Game.UI.Components;
using Game.UI.Providers;
using Game.UI.Windows;
using Injection;
using Utils;
using Utils.Collections;

namespace Game.UI.Controllers
{
  public class UIWindowReference
  {
    private readonly Lifetime.Definition _definition;

    public UIWindowReference(Lifetime.Definition definition)
    {
      _definition = definition;
    }

    public void Close()
    {
      _definition.Terminate();
    }
  }

  public class UIWindowController : UIController
  {
    [Inject]
    private IInjector _injector;
    [Inject]
    private UISceneController _sceneController;

    private Dictionary<Type, string> _map = new Dictionary<Type, string>();
    private List<UIWindow> _opened = new List<UIWindow>();
    private LinkedQueue<Action<Action>> _queue = new LinkedQueue<Action<Action>>();
    private bool _inOpenProcess;
    private Transform _transform;

    public UIWindowReference Open<T>(Action<UIWindow> onOpen = null) where T : UIWindow
    {
      var definition = Lifetime.Define(Lifetime);
      var shell = new UIWindowReference(definition);
      Enqueue(typeof(T), onOpen, definition);
      return shell;
    }

    protected override void Initialize()
    {
      foreach (var windowMap in new UIWindowsProvider().GetWindows())
      {
        _map[windowMap.Type] = windowMap.Path;
        _injector.Map(windowMap.Type).ToFactory(windowMap.Type);
      }
    }

    private void Enqueue(Type type, Action<UIWindow> onOpen, Lifetime.Definition lifetimeDefinition)
    {
      var intersectLifetime = Lifetime.Intersection(lifetimeDefinition.Lifetime, Lifetime);
      Action<Action> action = (callback) =>
      {
        var path = _map[type];
        Context.ResourceManager.GetPrefab(path).LoadAsync(intersectLifetime.Lifetime, result =>
        {
          var windowMediator = (UIWindow)_injector.Get(type);
          var windowComponent = result.Instantiate<UIWindowComponent>();
          GameObject.DontDestroyOnLoad(windowComponent.gameObject);
          _injector.Inject(windowMediator);
          _sceneController.SceneComponent.WindowsRoot.AddChild(windowComponent.transform);
          windowComponent.gameObject.AddComponent<SignalMonoBehaviour>().DestroySignal.Subscribe(intersectLifetime.Lifetime, intersectLifetime.Terminate);
          MethodInvoker<UIWindow, InitializeAttribute>.Invoke(windowMediator, intersectLifetime, windowComponent);
          _opened.Add(windowMediator);
          MethodInvoker<UIWindow, WindowOpenAttribute>.Invoke(windowMediator);
          if (onOpen != null) onOpen(windowMediator);
          callback();
          intersectLifetime.Lifetime.AddAction(() =>
          {
            MethodInvoker<UIWindow, WindowCloseAttribute>.Invoke(windowMediator);
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