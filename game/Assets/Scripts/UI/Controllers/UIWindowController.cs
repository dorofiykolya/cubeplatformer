using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

    public UIWindowReference(Lifetime.Definition definition, bool isFullscreen)
    {
      _definition = definition;
      IsFullscreen = isFullscreen;
    }

    public bool IsFullscreen { get; private set; }

    public void Close()
    {
      _definition.Terminate();
    }
  }

  public class UIWindowController : UIController
  {
    private class UIWindowContext
    {
      private readonly UIWindowReference _reference;
      private readonly Lifetime.Definition _definition;

      public UIWindowContext(UIWindowReference reference, Lifetime.Definition definition)
      {
        _reference = reference;
        _definition = definition;
      }

      public Action<Action> Factory;

      public Lifetime Lifetime
      {
        get { return _definition.Lifetime; }
      }

      public UIWindowReference Reference
      {
        get { return _reference; }
      }
    }

    [Inject]
    private IInjector _injector;
    [Inject]
    private UISceneController _sceneController;

    private readonly Dictionary<Type, UIWindowMap> _map = new Dictionary<Type, UIWindowMap>();
    private readonly List<UIWindowContext> _opened = new List<UIWindowContext>();
    private readonly LinkedQueue<UIWindowContext> _queue = new LinkedQueue<UIWindowContext>();
    private Signal _onChanged;
    private bool _inOpenProcess;

    protected override void OnPreinitialize()
    {
      _onChanged = new Signal(Lifetime);
    }

    public UIWindowReference Open<T>() where T : UIWindow
    {
      return Open<T>(null, null);
    }

    public UIWindowReference Open<T>(UIWindowData data) where T : UIWindow
    {
      return Open<T>(null, data);
    }

    public UIWindowReference Open<T>(Action<UIWindow> onOpen) where T : UIWindow
    {
      return Open<T>(onOpen, null);
    }

    public UIWindowReference Open<T>(Action<UIWindow> onOpen, UIWindowData data) where T : UIWindow
    {
      var definition = Lifetime.Define(Lifetime);
      var reference = new UIWindowReference(definition, _map[typeof(T)].IsFullscreen);
      var context = new UIWindowContext(reference, definition);
      Enqueue(typeof(T), onOpen, context, data);
      return reference;
    }

    public IEnumerable<UIWindowReference> Opened
    {
      get { return _opened.Select(w => w.Reference).ToList(); }
    }

    public IEnumerable<UIWindowReference> Queue
    {
      get { return _queue.Select(w => w.Reference).ToArray(); }
    }

    protected override void OnInitialize()
    {
      foreach (var windowMap in new UIWindowsProvider().GetWindows())
      {
        _map[windowMap.Type] = windowMap;
        _injector.Map(windowMap.Type).ToFactory(windowMap.Type);
      }
    }

    private void Enqueue(Type type, Action<UIWindow> onOpen, UIWindowContext context, UIWindowData data)
    {
      var intersectLifetime = Lifetime.Intersection(context.Lifetime, Lifetime);
      context.Factory = callback =>
      {
        var path = _map[type].Path;
        Context.ResourceManager.GetPrefab(path).LoadAsync(intersectLifetime.Lifetime, result =>
        {
          var windowMediator = (UIWindow)_injector.Get(type);
          var windowComponent = result.Instantiate<UIWindowComponent>();

          intersectLifetime.Lifetime.AddAction(() =>
          {
            MethodInvoker<UIWindow, WindowCloseAttribute>.Invoke(windowMediator);
            _opened.Remove(context);
            result.Release(windowComponent);
            result.Collect();

            _onChanged.Fire();
          });

          GameObject.DontDestroyOnLoad(windowComponent.gameObject);
          _injector.Inject(windowMediator);
          _sceneController.SceneComponent.WindowsRoot.AddChild(windowComponent.transform);
          windowComponent.gameObject.AddComponent<SignalMonoBehaviour>().DestroySignal.Subscribe(intersectLifetime.Lifetime, intersectLifetime.Terminate);
          MethodInvoker<UIWindow, InitializeAttribute>.Invoke(windowMediator, intersectLifetime, windowComponent, data);
          _opened.Add(context);
          MethodInvoker<UIWindow, WindowOpenAttribute>.Invoke(windowMediator);
          if (onOpen != null) onOpen(windowMediator);
          callback();
          _onChanged.Fire();
        });
      };

      _queue.Enqueue(context);
      intersectLifetime.Lifetime.AddAction(() =>
      {
        _queue.Remove(context);
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
        _queue.Dequeue().Factory(() =>
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

    public void SubscribeOnChanged(Lifetime lifetime, Action listener)
    {
      _onChanged.Subscribe(lifetime, listener);
    }
  }
}