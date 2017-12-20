using Game.UI.HUDs;
using Injection;

namespace Game.UI.Controllers
{
  public class UIHUDGameStateController : UIController
  {
    [Inject]
    private GameStateController _stateController;
    [Inject]
    private UIHUDController _hudController;

    protected override void OnInitialize()
    {
      Map<UIHUDClassicPlayMode>(GameState.ClassicPlayMode);
      Map<UIHUDSceneMainManu>(GameState.Menu);
    }

    private void Map<T>(GameState state) where T : UIHUD
    {
      _stateController.SubscribeOnEnter(Lifetime, state, exitState =>
      {
        var reference = _hudController.Open<T>();
        _stateController.SubscribeOnExit(Lifetime, state, enterState =>
        {
          reference.Close();
        });
      });
    }
  }
}