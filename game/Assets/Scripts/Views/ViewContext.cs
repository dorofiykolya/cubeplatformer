using System.Linq;
using Game.Views.Providers;
using Injection;

namespace Game.Views
{
  public class ViewContext
  {
    private readonly GameContext _context;
    private Injector _injector;

    public ViewContext(GameContext context, Injector injector)
    {
      _context = context;
      _injector = new Injector(injector);
      _injector.Map<ViewContext>().ToValue(this);

      var controllers = new ViewControllersProvider().Providers(this).ToList();
      foreach (var controller in controllers)
      {
        _injector.Map(controller.GetType()).ToValue(controller);
      }
      foreach (var controller in controllers)
      {
        _injector.Inject(controller);
      }
      foreach (var controller in controllers)
      {
        controller.PreinitializeController();
      }
      foreach (var controller in controllers)
      {
        controller.InitializeController();
      }
    }

    public GameContext Context { get { return _context; } }
  }
}