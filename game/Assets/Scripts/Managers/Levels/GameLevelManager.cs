using Game.Managers;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
  public class GameLevelManager : GameManager
  {
    [Inject]
    private GamePersistanceManager _persistanceManager;
    [Inject]
    private GameSceneManager _gameSceneManager;

    private GameLevels _classicLevels;
    private GameClassicLevelInfo _classicLevel;

    protected override void Initialize()
    {
      _classicLevels = Resources.Load<GameLevels>("GameLevels");
    }

    public void ResumeClassic()
    {
      DestroyLastLevel();
      var levelData = _classicLevels.GetLevel(_persistanceManager.LastClassicLevel);
      _gameSceneManager.LoadScene(levelData.Scene, LoadSceneMode.Single, scene =>
      {
        _classicLevel = new GameClassicLevelInfo();
        _classicLevel.Scene = scene;
        if (levelData.EnvironmentPrefab) _classicLevel.Envorinment = Object.Instantiate(levelData.EnvironmentPrefab);
        if (levelData.LevelPrefab) _classicLevel.Level = Object.Instantiate(levelData.LevelPrefab);
        if (levelData.DataType == GameLevelDataType.StringFormat)
        {
          _classicLevel.Level = LevelStringBuilder.CreateLevel(levelData.LevelStringData, levelData.Preset);
        }
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
        if (_classicLevel.Envorinment) GameObject.Destroy(_classicLevel.Envorinment);
        if (_classicLevel.Level) GameObject.Destroy(_classicLevel.Level);
        _classicLevel = null;
      }
    }
  }
}