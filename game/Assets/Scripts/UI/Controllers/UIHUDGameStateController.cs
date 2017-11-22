using UnityEngine;
using System.Collections;
using Game.UI.HUDs;
using Game.UI.Windows;
using Injection;

namespace Game.UI.Controllers
{
  public class UIHUDGameStateController : UIController
  {
    [Inject]
    private GameStateManager _stateManager;
    [Inject]
    private UIHUDController _hudController;

    protected override void OnInitialize()
    {
      Map<UIHUDClassicPlayMode>(GameState.ClassicPlayMode);
      //Map<UIHUDMainMenu>(GameState.Menu);
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