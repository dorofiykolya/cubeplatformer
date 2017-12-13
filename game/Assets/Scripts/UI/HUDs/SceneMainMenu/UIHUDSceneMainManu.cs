using System;
using System.Linq;
using Game.Inputs;
using Game.Managers;
using Game.UI.Components;
using Injection;
using Utils;

namespace Game.UI.HUDs
{
  public class UIHUDSceneMainManu : UIHUD
  {
    [Inject]
    private GameContext _gameContext;
    [Inject]
    private GameLevelManager _levelManager;

    private Lifetime.Definition _lifeDefinition;

    public override Type ComponentType
    {
      get { return null; }
    }

    protected override void Initialize()
    {

    }

    protected override void OnOpen()
    {
      _lifeDefinition = Lifetime.Define(Lifetime);
      var sceneManager = _gameContext.Managers.Get<GameSceneManager>();
      sceneManager.LoadScene(GameScenes.MainMenu, UnityEngine.SceneManagement.LoadSceneMode.Additive, (scene) =>
      {
        _lifeDefinition.Lifetime.AddAction(() =>
        {
          sceneManager.UnloadScene(GameScenes.MainMenu);
        });

        var gameObject = scene.GetRootGameObjects().FirstOrDefault(t => t.GetComponent<UIMainMenuComponent>() != null);
        sceneManager.SetActive(scene);
        var component = gameObject.GetComponent<UIMainMenuComponent>();
        var navigation = component.Navigation;

        var input = new MainMenuInputContext(_gameContext, Lifetime, _gameContext.InputContext.Current);

        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Horizontal, InputPhase.Begin, e =>
        {
          if (e.Value > 0) navigation.GoToRight();
          else navigation.GoToLeft();
        });
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Vertical, InputPhase.Begin, e =>
        {
          if (e.Value > 0) navigation.GoToTop();
          else navigation.GoToBottom();
        });
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Cancel, InputPhase.Begin, e => { navigation.GoToBack(); });
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Action, InputPhase.Begin, e =>
        {
          var currentMenuId = navigation.Current.Id;
          switch (currentMenuId)
          {
            case MainMenuId.Start:
              _levelManager.ResumeClassic();
              break;
          }
        });
      });
    }

    protected override void OnClose()
    {
      _lifeDefinition.Terminate();
    }

    private class MainMenuInputContext : InputContext
    {
      public MainMenuInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
      {
      }
    }
  }
}
