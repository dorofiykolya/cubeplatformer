using Assets.Scripts.UI.HUDs.MainMenu;
using Game.Inputs;
using Game.Controllers;
using Game.UI.HUDs;
using Injection;

namespace Game.UI.Windows
{
  public class UIHUDMainMenu : UIHUD<UIHUDMainManuComponent>
  {
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private GameContext _gameContext;

    private UIHUDMainMenuInputContext _inputContext;

    protected override void Initialize()
    {
      UIHUDMainManuComponent menuComponent = Component;
      if (menuComponent)
      {
        menuComponent.OnClassicClick.Subscribe(Lifetime, () =>
        {
          _levelController.LoadLevel(0);
          Close();
        });
        menuComponent.OnInfinityClick.Subscribe(Lifetime, () => _levelController.LoadLevel(0));
      }
      _inputContext = new UIHUDMainMenuInputContext(_gameContext, Lifetime, _gameContext.InputContext);
      _inputContext.Subscribe(Lifetime, GameInput.Up, InputPhase.End, InputUpdate.Update, evt => menuComponent.Up());
      _inputContext.Subscribe(Lifetime, GameInput.Down, InputPhase.End, InputUpdate.Update, evt => menuComponent.Down());
      _inputContext.Subscribe(Lifetime, GameInput.Submit, InputPhase.End, InputUpdate.Update, evt => menuComponent.Submit());
    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {

    }
  }
}