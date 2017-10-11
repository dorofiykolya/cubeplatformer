using Assets.Scripts.UI.HUDs.MainMenu;
using Game.Inputs;
using Game.Managers;
using Game.UI.HUDs;
using Injection;

namespace Game.UI.Windows
{
  public class UIHUDMainMenu : UIHUD<UIHUDMainManuComponent>
  {
    [Inject]
    private GameLevelManager _levelManager;
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
          _levelManager.ResumeClassic();
          Close();
        });
        menuComponent.OnInfinityClick.Subscribe(Lifetime, _levelManager.ResumeInfinity);
      }
      _inputContext = new UIHUDMainMenuInputContext(_gameContext, Lifetime, _gameContext.InputContext);
      _inputContext.Subscribe(Lifetime, GameInput.Vertical, InputPhase.End, evt =>
      {
        if (evt.Value > 0f) menuComponent.Up();
        else menuComponent.Down();
      });
      _inputContext.Subscribe(Lifetime, GameInput.Submit, InputPhase.End, evt => menuComponent.Submit());
    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {

    }
  }
}