using System.Linq;
using Game.UI.Controllers;
using Game.UI.Providers;
using Injection;
using Utils;

namespace Game.UI
{
  public class UIContext
  {
    private readonly Lifetime _lifetime;
    private readonly GameContext _context;

    public UIContext(GameContext context, Injector contextInjector)
    {
      _context = context;
      _lifetime = Lifetime.Define(context.Lifetime).Lifetime;

      var injector = new Injector(contextInjector);
      injector.Map<Lifetime>().ToValue(_lifetime);
      injector.Map<UIContext>().ToValue(this);

      var controllers = new UIContextControllersProvider().Provider(this).ToList();
      foreach (var controller in controllers)
      {
        injector.Map(controller.GetType()).ToValue(controller);
      }
      foreach (var controller in controllers)
      {
        injector.Inject(controller);
      }
      foreach (var controller in controllers)
      {
        ControllerInitializer.Preinitialize(controller);
      }

      foreach (var controller in controllers)
      {
        ControllerInitializer.Initialize(controller);
      }
    }

    public Lifetime Lifetime { get { return _lifetime; } }
    public GameContext Context { get { return _context; } }
  }
}