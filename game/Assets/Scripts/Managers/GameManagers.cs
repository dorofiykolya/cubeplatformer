using System.Collections.Generic;
using System.Linq;
using Game.Providers;
using Injection;
using Utils;

namespace Game.Managers
{
  public class GameManagers
  {
    private readonly IInjector _injector;

    public GameManagers(Lifetime lifetime, GameContext context, Injector injector, GameManagersProvider provider)
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

    public T Get<T>() where T : GameManager
    {
      return _injector.Get<T>();
    }
  }
}