using System;
using UnityEngine;
using System.Collections;
using Game.Managers;
using Game.UI.Components;
using Injection;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.UI.Controllers
{
  public class UISceneController : UIController
  {
    [Inject]
    private GameSceneManager _gameSceneManager;
    [Inject]
    private GameStateManager _stateManager;

    private Signal _onReady;
    private Signal _onUnload;
    private UISceneComponent _sceneComponent;

    protected override void Preinitialize()
    {
      _onReady = new Signal(Lifetime);
      _onUnload = new Signal(Lifetime);
    }

    protected override void Initialize()
    {
      _gameSceneManager.LoadScene("UI", LoadSceneMode.Additive, SceneLoadedHandler);
    }

    public void SubscribeOnSceneReady(Lifetime lifetime, Action listener)
    {
      _onReady.Subscribe(lifetime, listener);
    }

    public void SubscribeOnSceneUnLoad(Lifetime lifetime, Action listener)
    {
      _onUnload.Subscribe(lifetime, listener);
    }

    public UISceneComponent SceneComponent
    {
      get { return _sceneComponent; }
    }

    private void SceneLoadedHandler(Scene scene)
    {
      foreach (var rootGameObject in scene.GetRootGameObjects())
      {
        GameObject.DontDestroyOnLoad(rootGameObject);
        var sceneComponent = rootGameObject.GetComponent<UISceneComponent>();
        if (sceneComponent != null)
        {
          _sceneComponent = sceneComponent;
          _sceneComponent.gameObject.AddComponent<SignalMonoBehaviour>().DestroySignal.Subscribe(Lifetime, () =>
          {
            _sceneComponent = null;
            _onUnload.Fire();
          });
          _onReady.Fire();
          return;
        }
      }
      throw new Exception();
    }
  }
}