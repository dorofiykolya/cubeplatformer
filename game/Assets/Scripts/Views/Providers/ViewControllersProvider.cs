using System.Collections.Generic;
using Game.Views.Controllers;

namespace Game.Views.Providers
{
  public class ViewControllersProvider
  {
    public IEnumerable<ViewController> Providers(ViewContext viewContext)
    {
      yield return new ViewPreloaderController();
    }
  }
}