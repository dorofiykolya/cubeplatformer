using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.Managers
{
  public class GameSceneManager : GameManager
  {
    private Signal<Scene> _onSceneLoaded;
    private Signal<string> _onSceneUnLoaded;
    private Dictionary<string, SceneController> _map = new Dictionary<string, SceneController>();

    protected override void OnInitialize()
    {
      _onSceneLoaded = new Signal<Scene>(Lifetime);
      _onSceneUnLoaded = new Signal<string>(Lifetime);
    }

    public void SubscribeOnSceneLoaded(Lifetime lifetime, Action<Scene> listener)
    {
      _onSceneLoaded.Subscribe(lifetime, listener);
    }

    public void SubscribeOnSceneUnLoaded(Lifetime lifetime, Action<string> listener)
    {
      _onSceneUnLoaded.Subscribe(lifetime, listener);
    }

    public SceneController Get(string name)
    {
      SceneController controller;
      if (!_map.TryGetValue(name, out controller))
      {
        _map[name] = controller = new SceneController(name, Lifetime, Context);
        controller.SubscribeOnLoaded(Lifetime, _onSceneLoaded.Fire);
        controller.SubscribeOnUnLoaded(Lifetime, _onSceneUnLoaded.Fire);
        Lifetime.AddAction(() => _map.Remove(name));
      }
      return controller;
    }
  }
}