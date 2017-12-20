using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.Controllers
{
  public class GameSceneController : GameController
  {
    private Signal<Scene> _onSceneLoaded;
    private Signal<Scene> _onSceneUnloaded;

    protected override void OnInitialize()
    {
      _onSceneLoaded = new Signal<Scene>(Lifetime);
      _onSceneUnloaded = new Signal<Scene>(Lifetime);
    }

    public void LoadScene(string name, LoadSceneMode mode, Action<Scene> onComplete = null)
    {
      Context.StartCoroutine(Lifetime, LoadSceneAsync(name, mode, onComplete));
    }

    public void UnloadScene(string name, Action<Scene> onComplete = null)
    {
      Context.StartCoroutine(Lifetime, UnloadSceneAsync(name, onComplete));
    }

    public void SubscribeOnSceneLoaded(Lifetime lifetime, Action<Scene> listener)
    {
      _onSceneLoaded.Subscribe(lifetime, listener);
    }

    private IEnumerator UnloadSceneAsync(string name, Action<Scene> onComplete)
    {
      var definition = Lifetime.Define(Lifetime);
      Context.Preloader.Open(definition.Lifetime);
      yield return new WaitForSeconds(1f);
      var scene = SceneManager.GetSceneByName(name);
      if (scene.isLoaded)
      {
        foreach (var gameObject in scene.GetRootGameObjects())
        {
          GameObject.Destroy(gameObject);
        }
        var async = SceneManager.UnloadSceneAsync(name);
        yield return async;
      }
      yield return new WaitForSeconds(1f);
      if (onComplete != null)
      {
        onComplete(scene);
      }
      _onSceneLoaded.Fire(scene);
      definition.Terminate();
    }

    private IEnumerator LoadSceneAsync(string name, LoadSceneMode mode, Action<Scene> onComplete)
    {
      var definition = Lifetime.Define(Lifetime);
      Context.Preloader.Open(definition.Lifetime);
      yield return new WaitForSeconds(1f);
      var async = SceneManager.LoadSceneAsync(name, mode);
      yield return async;
      yield return new WaitForSeconds(1f);
      var scene = SceneManager.GetSceneByName(name);
      if (onComplete != null)
      {
        onComplete(scene);
      }
      _onSceneLoaded.Fire(scene);
      definition.Terminate();
    }

    public bool SetActive(Scene scene)
    {
      return SceneManager.SetActiveScene(scene);
    }
  }
}