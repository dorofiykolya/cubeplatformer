using Game.Controllers;
using Game.UI.Controllers;
using System.Collections.Generic;

namespace Game.UI.Providers
{
  public class UIContextControllersProvider
  {
    public IEnumerable<UIController> Provider(UIContext context)
    {
      yield return new UIMainMenuController();
      yield return new UIHUDController();
      yield return new UISceneController();
      yield return new UIWindowController();
      yield return new UIHUDGameStateController();
      yield return new UIHUDPreloaderController();
      yield return new UITutorialController();
    }

  }
}