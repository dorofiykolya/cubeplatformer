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

    protected override void Initialize()
    {
      _sceneController.SubscribeOnSceneReady(Lifetime, InitializeMenu);
    }

    private void InitializeMenu()
    {
      var lt = Context.UIContext.Windows.Open<UIMainMenuWindow>((w) => { });
    }
  }
}