using System.Linq;
using Game.Views.Controllers;
using Game.Views.Providers;
using Injection;
using Utils;

namespace Game.Views
{
  public class ViewContext
  {
    private readonly GameContext _context;
    private readonly Lifetime _lifetime;

    public ViewContext(GameContext context, Injector contextInjector)
    {
      _lifetime = Lifetime.Define(context.Lifetime).Lifetime;
      _context = context;

      var injector = new Injector(contextInjector);
      injector.Map<Lifetime>().ToValue(_lifetime);
      injector.Map<ViewContext>().ToValue(this);

      var controllers = new ViewControllersProvider().Providers(this).ToList();
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
        PreInitializer<Controller>.Preinitialize(controller);
      }

      foreach (var controller in controllers)
      {
        Initializer<Controller>.Initialize(controller);
      }
    }

    public Lifetime Lifetime { get { return _lifetime; } }
    public GameContext Context { get { return _context; } }
  }
}