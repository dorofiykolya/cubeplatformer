using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.Managers
{
  public class GameSceneManager : GameManager
  {
    public void LoadScene(string name, LoadSceneMode mode, Action<Scene> onComplete = null)
    {
      Context.StartCoroutine(Lifetime, LoadSceneAsync(name, mode, onComplete));
    }

    private IEnumerator LoadSceneAsync(string name, LoadSceneMode mode, Action<Scene> onComplete)
    {
      var definition = Lifetime.Define(Lifetime);
      Context.Preloader.Open(definition.Lifetime);
      yield return new WaitForSeconds(1f);
      var async = SceneManager.LoadSceneAsync(name, mode);
      yield return async;
      yield return new WaitForSeconds(1f);
      if (onComplete != null)
      {
        onComplete(SceneManager.GetSceneByName(name));
      }
      definition.Terminate();
    }
  }
}