using System;
using UnityEngine;
using System.Collections;
using Game.Controllers;
using Game.UI.Components;
using Injection;
using UnityEngine.SceneManagement;
using Utils;
using Game.Commands;
using Game.Messages;

namespace Game.UI.Controllers
{
  public class UISceneController : UIController
  {
    [Inject]
    private GameSceneController _gameSceneController;
    [Inject]
    private GameStateController _stateController;

    private Signal _onReady;
    private Signal _onUnload;
    private UISceneComponent _sceneComponent;

    protected override void OnPreinitialize()
    {
      _onReady = new Signal(Lifetime);
      _onUnload = new Signal(Lifetime);
    }

    protected override void OnInitialize()
    {
      Context.CommandMap.Map<StartMessage>().RegisterCommand(lifetime => new StartGameCommand(this), true);
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

    private class StartGameCommand : ICommand
    {
      private UISceneController _uISceneController;

      public StartGameCommand(UISceneController uISceneController)
      {
        _uISceneController = uISceneController;
      }

      public void Execute()
      {
        _uISceneController._gameSceneController.LoadScene(GameScenes.UI, LoadSceneMode.Additive, _uISceneController.SceneLoadedHandler);
      }
    }
  }
}