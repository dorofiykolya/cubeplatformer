using Game.Managers;
using Game.UI.Windows;
using Injection;

namespace Game.UI.Controllers
{
  public class UIMainMenuController : UIController
  {
    [Inject]
    private GameLevelManager _levelManager;
    [Inject]
    private UISceneController _sceneController;
    [Inject]
    private UIWindowController _windowController;
    [Inject]
    private GameStateManager _stateManager;

    protected override void OnInitialize()
    {
      _sceneController.SubscribeOnSceneReady(Lifetime, InitializeMenu);
    }

    private void InitializeMenu()
    {
      _stateManager.Current = GameState.Menu;
    }
  }
}