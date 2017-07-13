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

    protected override void Preinitialize()
    {
      _onLoaded = new Signal<GameClassicLevelInfo>(Lifetime);
      _onUnload = new Signal<GameClassicLevelInfo>(Lifetime);
    }

    protected override void Initialize()
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
      _gameSceneManager.LoadScene(levelData.Scene, LoadSceneMode.Single, scene =>
      {
        _classicLevel = new GameClassicLevelInfo();
        _classicLevel.Scene = scene;
        if (levelData.EnvironmentPrefab) _classicLevel.Envorinment = UnityEngine.Object.Instantiate(levelData.EnvironmentPrefab);
        if (levelData.LevelPrefab) _classicLevel.Level = UnityEngine.Object.Instantiate(levelData.LevelPrefab);
        if (levelData.DataType == GameLevelDataType.StringFormat)
        {
          _classicLevel.Level = LevelStringBuilder.CreateLevel(levelData.LevelStringData, levelData.Preset);
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

    private void DestroyLastLevel()
    {
      if (_classicLevel != null)
      {
        _onUnload.Fire(_classicLevel);
        if (_classicLevel.Envorinment) GameObject.Destroy(_classicLevel.Envorinment);
        if (_classicLevel.Level) GameObject.Destroy(_classicLevel.Level);
        _classicLevel = null;
      }
    }
  }
}