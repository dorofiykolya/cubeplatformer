using Game.Controllers;
using Game.UI.Windows;
using Injection;

namespace Game.UI.Controllers
{
  public class UIMainMenuController : UIController
  {
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private UISceneController _sceneController;
    [Inject]
    private UIWindowController _windowController;
    [Inject]
    private GameStateController _stateController;

    protected override void OnInitialize()
    {
      //_sceneController.SubscribeOnSceneReady(Lifetime, InitializeMenu);
    }

    private void InitializeMenu()
    {
      //_stateManager.Current = GameState.Menu;
    }
  }
}