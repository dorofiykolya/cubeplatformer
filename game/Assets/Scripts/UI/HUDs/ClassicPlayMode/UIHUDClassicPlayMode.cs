using Game.Managers;
using Injection;

namespace Game.UI.HUDs
{
  public class UIHUDClassicPlayMode : UIHUD<UIHUDClassicPlayModeComponent>
  {
    [Inject]
    private GameLevelManager _levelManager;

    protected override void Initialize()
    {
      Component.SubscribeOnClick(Lifetime, () =>
      {
        _levelManager.Unload();
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