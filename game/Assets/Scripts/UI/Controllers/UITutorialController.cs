using Game.Controllers;
using Game.UI.Windows;
using Injection;

namespace Game.UI.Controllers
{
  public class UITutorialController : UIController
  {
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private UIWindowController _windowController;

    protected override void OnInitialize()
    {
      _levelController.SubscribeOnLoaded(Lifetime, info => { _windowController.Open<UITutorialWindow>(); });
    }
  }
}
