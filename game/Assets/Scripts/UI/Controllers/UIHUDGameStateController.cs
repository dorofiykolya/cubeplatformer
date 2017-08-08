using UnityEngine;
using System.Collections;
using Game.UI.HUDs;
using Injection;

namespace Game.UI.Controllers
{
  public class UIHUDGameStateController : UIController
  {
    [Inject]
    private GameStateManager _stateManager;
    [Inject]
    private UIHUDController _hudController;

    protected override void Initialize()
    {
      Map<UIHUDClassicPlayMode>(GameState.ClassicPlayMode);
    }

    private void Map<T>(GameState state) where T : UIHUD
    {
      _stateManager.SubscribeOnEnter(Lifetime, state, exitState =>
      {
        var reference = _hudController.Open<T>();
        _stateManager.SubscribeOnExit(Lifetime, state, enterState =>
        {
          reference.Close();
        });
      });
    }
  }
}