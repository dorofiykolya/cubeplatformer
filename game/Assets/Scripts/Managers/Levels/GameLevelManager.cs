using System;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.Managers
{
  public class GameLevelManager : GameManager
  {
    [Inject]
    private GamePersistanceManager _persistanceManager;
    [Inject]
    private GameSceneManager _gameSceneManager;

    private Signal<GameClassicLevelInfo> _onLoaded;
    private Signal<GameClassicLevelInfo> _onUnload;
    private GameLevels _classicLevels;
    private GameClassicLevelInfo _classicLevel;

    protected override void OnPreinitialize()
    {
      _onLoaded = new Signal<GameClassicLevelInfo>(Lifetime);
      _onUnload = new Signal<GameClassicLevelInfo>(Lifetime);
    }

    protected override void OnInitialize()
    {
      _classicLevels = Resources.Load<GameLevels>("GameLevels");
    }

    public void SubscribeOnLoaded(Lifetime lifetime, Action<GameClassicLevelInfo> listener)
    {
      _onLoaded.Subscribe(lifetime, listener);
    }

    public void SubscribeOnUnloaded(Lifetime lifetime, Action<GameClassicLevelInfo> listener)
    {
      _onUnload.Subscribe(lifetime, listener);
    }

    public void ResumeClassic()
    {
      DestroyLastLevel();
      var levelData = _classicLevels.GetLevel(_persistanceManager.LastClassicLevel);
      _gameSceneManager.LoadScene(levelData.Scene.SceneName, LoadSceneMode.Single, scene =>
      {
        _classicLevel = new GameClassicLevelInfo();
        _classicLevel.Scene = scene;
        if (levelData.DataType == GameLevelDataType.StringFormat)
        {
          _classicLevel.Level = levelData.LevelStringData.Asset.text;
          _classicLevel.Preset = levelData.Preset;
        }
        _onLoaded.Fire(_classicLevel);
      });
    }

    public void NextClassic()
    {
      var currentLevel = _persistanceManager.LastClassicLevel;
      var data = _classicLevels.GetLevel(currentLevel + 1);
      if (data != null)
      {
        _persistanceManager.LastClassicLevel++;
        ResumeClassic();
      }
    }

    public void ResumeInfinity()
    {

    }

    public void Unload()
    {
      DestroyLastLevel();
    }

    private void DestroyLastLevel()
    {
      if (_classicLevel != null)
      {
        _onUnload.Fire(_classicLevel);
        foreach (var gameObject in _classicLevel.Scene.GetRootGameObjects())
        {
          GameObject.DestroyImmediate(gameObject);
        }
        if (_classicLevel.Envorinment) GameObject.Destroy(_classicLevel.Envorinment.gameObject);
        SceneManager.UnloadSceneAsync(_classicLevel.Scene.name);
        _classicLevel = null;
      }
    }
  }
}