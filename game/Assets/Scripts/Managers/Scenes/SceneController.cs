using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.Managers
{
  public class SceneController
  {
    public enum SceneStatus
    {
      Unloaded = 0,
      Loaded = 1,
      Loading = 2,
      Unloading = 3
    }

    private readonly Signal<Scene> _onLoaded;
    private readonly Signal<string> _onUnloaded;
    private readonly Lifetime _lifetime;
    private readonly GameContext _context;
    private readonly string _name;

    private Lifetime<bool>.Definition _loadDefinition;
    private SceneStatus _status;
    private Scene _scene;

    public SceneController(string name, Lifetime lifetime, GameContext context)
    {
      _name = name;
      _lifetime = lifetime;
      _context = context;
      _onLoaded = new Signal<Scene>(lifetime);
      _onUnloaded = new Signal<string>(lifetime);
    }

    public string Name { get { return _name; } }
    public SceneStatus Status { get { return _status; } }
    public Scene Scene { get { return _scene; } }

    public void Activate()
    {
      if (_status == SceneStatus.Loaded)
      {
        SceneManager.SetActiveScene(_scene);
      }
      else
      {
        throw new InvalidOperationException();
      }
    }

    public SceneController Load(bool withPreloader = true, bool activate = false)
    {
      if (_status == SceneStatus.Loaded)
      {
        throw new InvalidOperationException("scene already loaded: " + _name);
      }
      if (_status == SceneStatus.Loading)
      {
        throw new InvalidOperationException("scene loading: " + _name);
      }
      if (_status == SceneStatus.Unloading)
      {
        var definition = Lifetime.Define(_lifetime);
        _onUnloaded.Subscribe(definition.Lifetime, (scene) =>
        {
          Load(withPreloader);
          definition.Terminate();
        });
        return this;
      }

      _status = SceneStatus.Loading;
      _loadDefinition = Lifetime<bool>.Define(Lifetime<bool>.Eternal, "loading");
      _lifetime.AddAction(() => _loadDefinition.Terminate(false));
      _loadDefinition.Lifetime.AddAction((value) =>
      {
        if (_status == SceneStatus.Loaded)
        {
          _status = SceneStatus.Unloading;
          _context.StartCoroutine(_lifetime, UnloadSceneAsync(withPreloader, () =>
          {
            _status = SceneStatus.Unloaded;
            _onUnloaded.Fire(_name);
          }));
        }
        else if (_status == SceneStatus.Loading)
        {
          throw new InvalidOperationException("can not unload on loading scene");
        }
      });
      _context.StartCoroutine(_lifetime, LoadSceneAsync(_name, LoadSceneMode.Additive, withPreloader, (scene) =>
      {
        _scene = scene;
        _status = SceneStatus.Loaded;
        _onLoaded.Fire(scene);
        if (activate)
        {
          Activate();
        }
      }));

      return this;
    }

    public void DestroyObjects()
    {
      if (_status == SceneStatus.Loaded)
      {
        foreach (var obj in _scene.GetRootGameObjects())
        {
          GameObject.Destroy(obj);
        }
      }
    }

    public SceneController Unload(bool withPreloader = true)
    {
      if (_loadDefinition != null)
      {
        _loadDefinition.Terminate(withPreloader);
      }
      return this;
    }

    private IEnumerator LoadSceneAsync(string name, LoadSceneMode mode, bool withPreloader, Action<Scene> onComplete)
    {
      yield return null;

      var definition = Lifetime.Define(_lifetime);
      if (withPreloader)
      {
        _context.Preloader.Open(definition.Lifetime);
      }
      yield return new WaitForSeconds(1f);
      var async = SceneManager.LoadSceneAsync(name, mode);
      yield return async;
      yield return new WaitForSeconds(1f);
      var scene = SceneManager.GetSceneByName(name);
      if (onComplete != null)
      {
        onComplete(scene);
      }
      definition.Terminate();
    }

    private IEnumerator UnloadSceneAsync(bool withPreloader, Action onComplete)
    {
      yield return null;

      var definition = Lifetime.Define(_lifetime);
      if (withPreloader)
      {
        _context.Preloader.Open(definition.Lifetime);
      }
      yield return new WaitForSeconds(1f);
      foreach (var obj in _scene.GetRootGameObjects())
      {
        GameObject.Destroy(obj.gameObject);
      }
      var async = SceneManager.UnloadSceneAsync(_scene);
      yield return async;
      yield return new WaitForSeconds(1f);
      if (onComplete != null)
      {
        onComplete();
      }
      definition.Terminate();
    }

    public void SubscribeOnUnLoaded(Lifetime lifetime, Action<string> listener)
    {
      _onUnloaded.Subscribe(lifetime, listener);
    }

    public void SubscribeOnLoaded(Lifetime lifetime, Action<Scene> listener)
    {
      _onLoaded.Subscribe(lifetime, listener);
    }
  }
}
