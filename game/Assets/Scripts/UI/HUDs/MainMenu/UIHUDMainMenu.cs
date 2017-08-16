using Game.Managers;
using Game.UI.HUDs;
using Injection;

namespace Game.UI.Windows
{
  public class UIHUDMainMenu : UIHUD<UIHUDMainManuComponent>
  {
    [Inject]
    private GameLevelManager _levelManager;

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
    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {

    }
  }
}