using System.Collections.Generic;
using System.Linq;
using Game.Providers;
using Injection;
using Utils;

namespace Game.Managers
{
  public class GameManagers
  {
    public class Provider
    {
      private readonly List<GameManager> _list = new List<GameManager>();

      protected void Add(GameManager manager)
      {
        _list.Add(manager);
      }

      public GameManager[] ToArray()
      {
        return _list.ToArray();
      }
    }

    public GameManagers(Lifetime lifetime, GameContext context, Injector injector, GameManagersProvider provider)
    {
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
        ControllerInitializer.Preinitialize(manager);
      }
      foreach (var manager in managers)
      {
        ControllerInitializer.Initialize(manager);
      }
    }
  }
}