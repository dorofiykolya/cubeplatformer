using Game.Managers;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
  public class GameLevelManager : GameManager
  {
    [Inject]
    private GamePersistanceManager _persistanceManager;
    [Inject]
    private GameSceneManager _gameSceneManager;

    private GameLevels _classicLevels;
    private GameClassicLevel _classicLevel;

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
        _classicLevel = new GameClassicLevel
        {
          Scene = scene,
          Envorinment = Object.Instantiate(levelData.EnvironmentPrefab),
          Level = Object.Instantiate(levelData.LevelPrefab)
        };
      });
    }

    public void ResumeInfinity()
    {

    }

    private void DestroyLastLevel()
    {
      if (_classicLevel != null)
      {
        GameObject.Destroy(_classicLevel.Envorinment);
        GameObject.Destroy(_classicLevel.Level);
        _classicLevel = null;
      }
    }
  }
}