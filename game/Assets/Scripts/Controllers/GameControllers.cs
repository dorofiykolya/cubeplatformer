using System.Collections.Generic;
using System.Linq;
using Game.Providers;
using Injection;
using Utils;

namespace Game.Controllers
{
  public class GameControllers
  {
    private readonly IInjector _injector;

    public GameControllers(Lifetime lifetime, GameContext context, Injector injector, GameControllersProvider provider)
    {
      _injector = injector;

      injector.Map<GameContext>().ToValue(context);
      injector.Map<Lifetime>().ToValue(lifetime);

      var managers = provider.Providers(context).ToList();
      foreach (var manager in managers)
      {
        injector.Map(manager.GetType()).ToValue(manager);
      }
      foreach (var manager in managers)
      {
        injector.Inject(manager);
      }
      foreach (var manager in managers)
      {
        PreInitializer<Controller>.Preinitialize(manager);
      }
      foreach (var manager in managers)
      {
        Initializer<Controller>.Initialize(manager);
      }
    }

    public T Get<T>() where T : GameController
    {
      return _injector.Get<T>();
    }
  }
}