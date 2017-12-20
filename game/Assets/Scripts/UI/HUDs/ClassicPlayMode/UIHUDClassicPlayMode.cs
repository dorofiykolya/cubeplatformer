using Game.Controllers;
using Injection;

namespace Game.UI.HUDs
{
  public class UIHUDClassicPlayMode : UIHUD<UIHUDClassicPlayModeComponent>
  {
    [Inject]
    private GameLevelController _levelController;

    protected override void Initialize()
    {
      Component.SubscribeOnClick(Lifetime, () =>
      {
        _levelController.Unload();
      });
    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {

    }
  }
}