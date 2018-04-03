using System;
using System.Linq;
using Game.Components.MenuNavigation;
using Game.Inputs;
using Game.Controllers;
using Game.UI.Components;
using Game.UI.Controllers;
using Game.UI.Windows;
using Injection;
using Utils;

namespace Game.UI.HUDs
{
  public class UIHUDSceneMainManu : UIHUD
  {
    [Inject]
    private GameContext _gameContext;
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private UIWindowController _windowController;

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
      var sceneManager = _gameContext.Controllers.Get<GameSceneController>();
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
        component.ActiveCameras(_windowController.Opened.All(w => !w.IsFullscreen));
        _windowController.SubscribeOnChanged(Lifetime, () => component.ActiveCameras(_windowController.Opened.All(w => !w.IsFullscreen)));

        navigation.SubscribeOnAction(_lifeDefinition.Lifetime, () =>
        {
          DoAction(navigation);
        });

        var input = new MainMenuInputContext(_gameContext, Lifetime, _gameContext.InputContext.Current);

        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Right, InputPhase.Begin, InputUpdate.Update, e => navigation.GoToRight());
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Left, InputPhase.Begin, InputUpdate.Update, e => navigation.GoToLeft());
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Up, InputPhase.Begin, InputUpdate.Update, e => navigation.GoToTop());
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Down, InputPhase.Begin, InputUpdate.Update, e => navigation.GoToBottom());
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Cancel, InputPhase.Begin, InputUpdate.Update, e => { navigation.GoToBack(); });
        input.Subscribe(_lifeDefinition.Lifetime, GameInput.Action, InputPhase.Begin, InputUpdate.Update, e =>
        {
          DoAction(navigation);
        });
      });
    }

    private void DoAction(MenuNavigationComponent navigation)
    {
      var currentMenuId = navigation.Current.Id;
      switch (currentMenuId)
      {
        case MainMenuId.Start:
          _levelController.LoadLevel(0);
          break;
        case MainMenuId.Settings:
          _lifeDefinition.Lifetime.AddAction(_windowController.Open<UISettingsWindow>().Close);
          break;
        default:
          var id = (int)currentMenuId;
          var def = Lifetime.Define(Lifetime);
          _gameContext.Preloader.Open(def.Lifetime);
          _lifeDefinition.Lifetime.AddAction(_windowController.Open<UILevelSelectWindow>((w)=> def.Terminate(), new UILevelSelectWindowData
          {
            Level = id
          }).Close);
          break;
      }
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
