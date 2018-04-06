using System;
using Game.Components;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.Controllers
{
  public class GameLevelController : GameController
  {
    [Inject]
    private GamePersistanceController _persistanceController;
    [Inject]
    private GameSceneController _gameSceneController;

    private Signal<CurrentLevelInfo> _onLoaded;
    private Signal<CurrentLevelInfo> _onUnload;
    private Signal _onReady;
    private GameLevels _gameLevels;
    private CurrentLevelInfo _currentLevel;
    private bool _isReady;

    protected override void OnPreinitialize()
    {
      _onLoaded = new Signal<CurrentLevelInfo>(Lifetime);
      _onUnload = new Signal<CurrentLevelInfo>(Lifetime);
      _onReady = new Signal(Lifetime);
    }

    protected override void OnInitialize()
    {
      _gameLevels = Resources.Load<GameLevels>("GameLevels");
      _isReady = true;
      _onReady.Fire();
    }

    public bool IsReady { get { return _isReady; } }

    public void SubscribeOnLoaded(Lifetime lifetime, Action<CurrentLevelInfo> listener)
    {
      _onLoaded.Subscribe(lifetime, listener);
    }

    public void SubscribeOnUnloaded(Lifetime lifetime, Action<CurrentLevelInfo> listener)
    {
      _onUnload.Subscribe(lifetime, listener);
    }

    public void SubscribeOnReady(Lifetime lifetime, Action listener)
    {
      if (IsReady) listener();
      else _onReady.Subscribe(lifetime, listener);
    }

    public GameLevels Levels { get { return _gameLevels; } }
    public CurrentLevelInfo Current { get { return _currentLevel; } }

    public void LoadLevel(int index, int subLevel)
    {
      DestroyLastLevel();
      var levelData = _gameLevels.GetLevel(index);
      var data = levelData.GetLevel(subLevel);
      _gameSceneController.LoadScene(data.Scene.SceneName, LoadSceneMode.Single, (scene) =>
      {
        if (data.DataType == GameLevelDataType.StringFormat)
        {
          var classicLevel = new GameClassicLevelInfo();
          classicLevel.Scene = scene;
          if (data.DataType == GameLevelDataType.StringFormat)
          {
            classicLevel.Level = data.LevelStringData.text;
            classicLevel.Preset = data.Preset;
          }

          _currentLevel = new CurrentLevelInfo(Lifetime, classicLevel);
          _currentLevel.Lifetime.AddAction(() =>
          {
            _onUnload.Fire(_currentLevel);
            foreach (var gameObject in classicLevel.Scene.GetRootGameObjects())
            {
              GameObject.DestroyImmediate(gameObject);
            }
            if (classicLevel.Envorinment) GameObject.Destroy(classicLevel.Envorinment.gameObject);
            SceneManager.UnloadSceneAsync(classicLevel.Scene.name);
          });

          _onLoaded.Fire(_currentLevel);
        }
        else
        {
          foreach (var gameObject in scene.GetRootGameObjects())
          {
            var levelController = gameObject.GetComponent<LevelControllerComponent>();
            if (levelController != null)
            {
              _currentLevel = new CurrentLevelInfo(Lifetime, levelController);
              levelController.Load(Context, _currentLevel.Lifetime);
              
              _currentLevel.Lifetime.AddAction(() =>
              {
                _onUnload.Fire(_currentLevel);
                if (scene.IsValid() && scene.isLoaded)
                {
                  foreach (var g in scene.GetRootGameObjects())
                  {
                    GameObject.DestroyImmediate(g);
                  }
                  SceneManager.UnloadSceneAsync(scene.name);
                }
              });
              _onLoaded.Fire(_currentLevel);
              break;
            }
          }
        }
      });
    }

    public void Unload()
    {
      DestroyLastLevel();
    }

    private void DestroyLastLevel()
    {
      if (_currentLevel != null)
      {
        var level = _currentLevel;
        _currentLevel = null;
        level.Terminate();
      }
    }
  }
}