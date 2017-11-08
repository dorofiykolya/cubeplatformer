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
    private Lifetime.Definition _levelDefinition;

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

      _levelDefinition = Lifetime.Define(Lifetime);

      var levelData = _classicLevels.GetLevel(_persistanceManager.LastClassicLevel);
      _gameSceneManager.Get(levelData.Scene.SceneName).Load(true, true).SubscribeOnLoaded(_levelDefinition.Lifetime, scene =>
      {
        var classicLevel = new GameClassicLevelInfo
        {
          Scene = scene
        };
        if (levelData.EnvironmentPrefab.Asset != null) classicLevel.Envorinment = UnityEngine.Object.Instantiate(levelData.EnvironmentPrefab.Asset);
        if (levelData.DataType == GameLevelDataType.StringFormat)
        {
          classicLevel.Level = levelData.LevelStringData.Asset.text;
          classicLevel.Preset = levelData.Preset;
        }
        _onLoaded.Fire(classicLevel);

        _levelDefinition.Lifetime.AddAction(() =>
        {
          if (classicLevel != null)
          {
            _onUnload.Fire(classicLevel);
            if (classicLevel.Envorinment) GameObject.Destroy(classicLevel.Envorinment.gameObject);
            _gameSceneManager.Get(classicLevel.Scene.name).Unload(true);
            classicLevel = null;
          }
        });
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
      if (_levelDefinition != null)
      {
        _levelDefinition.Terminate();
      }
    }
  }
}