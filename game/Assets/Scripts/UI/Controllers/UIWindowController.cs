using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.UI.Components;
using Game.UI.Providers;
using Injection;
using Utils;
using Utils.Collections;

namespace Game.UI.Controllers
{
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

    public Lifetime.Definition Open<T>(Action<UIWindow> onOpen) where T : UIWindow
    {
      var definition = Lifetime.Define(Lifetime);
      Enqueue(typeof(T), onOpen, definition);

      return definition;
    }

    protected override void Initialize()
    {
      foreach (var windowMap in new UIWindowsProvider().GetWindows())
      {
        _map[windowMap.Type] = windowMap.Path;
        _injector.Map(windowMap.Type).ToFactory(windowMap.Type);
      }
    }

    private void Enqueue(Type type, Action<UIWindow> onOpen, Lifetime.Definition lifetime)
    {
      var intersectLifetime = Lifetime.Intersection(lifetime.Lifetime, Lifetime);
      Action<Action> action = (callback) =>
      {
        var path = _map[type];
        Context.ResourceManager.GetPrefab(path).LoadAsync(intersectLifetime.Lifetime, result =>
        {
          var windowMediator = (UIWindow)_injector.Get(type);
          var windowComponent = result.Instantiate<UIWindowComponent>();
          _injector.Map<UIWindowComponent>().ToValue(windowComponent);
          _injector.Inject(windowMediator);
          _injector.Unmap<UIWindowComponent>();
          _sceneController.SceneComponent.WindowsRoot.AddChild(windowComponent.transform);
          windowComponent.gameObject.AddComponent<SignalMonoBehaviour>().DestroySignal.Subscribe(intersectLifetime.Lifetime, intersectLifetime.Terminate);
          MethodInvoker<UIWindow, InitializeAttribute>.Invoke(windowMediator, intersectLifetime);
          _opened.Add(windowMediator);
          MethodInvoker<UIWindow, WindowOpenAttribute>.Invoke(windowMediator);
          onOpen(windowMediator);
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