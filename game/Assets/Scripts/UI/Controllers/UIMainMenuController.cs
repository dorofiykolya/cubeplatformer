using System.Collections;
using Game.Managers;
using Game.UI.Components;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Game.UI.Controllers
{
  public class UIMainMenuController : UIController
  {
    [Inject]
    private GameSceneManager _gameSceneManager;
    [Inject]
    private GameLevelManager _levelManager;

    protected override void Initialize()
    {
      _gameSceneManager.LoadScene("UI", LoadSceneMode.Single, InitializeMenu);
    }

    private void InitializeMenu(Scene scene)
    {
      Assert2.IsTrue(scene.IsValid());
      UIMainManuComponent menuComponent = null;
      foreach (var gameObject in scene.GetRootGameObjects())
      {
        menuComponent = gameObject.GetComponentInChildren<UIMainManuComponent>();
        if (menuComponent != null)
        {
          break;
        }
      }

      if (menuComponent)
      {
        menuComponent.OnClassicClick.Subscribe(Lifetime, _levelManager.ResumeClassic);
        menuComponent.OnInfinityClick.Subscribe(Lifetime, _levelManager.ResumeInfinity);
      }
    }
  }
}