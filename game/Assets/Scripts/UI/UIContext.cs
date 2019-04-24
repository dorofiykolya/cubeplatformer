using System;
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
    private readonly IInjector _injector;

    public UIContext(GameContext context, Injector contextInjector)
    {
      _context = context;
      _lifetime = context.Lifetime.DefineNested("UIContext").Lifetime;

      _injector = new Injector(contextInjector);
    }

    public bool IsReady { get; private set; }
    public Lifetime Lifetime { get { return _lifetime; } }
    public GameContext Context { get { return _context; } }
    public UIWindowController Windows { get { return _injector.Get<UIWindowController>(); } }

    [Initialize]
    private void OnInitialize()
    {
      _injector.Map<Lifetime>().ToValue(_lifetime);
      _injector.Map<UIContext>().ToValue(this);

      var controllers = new UIContextControllersProvider().Provider(this).ToList();
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
        PreInitializer<Controller>.Preinitialize(controller);
      }

      foreach (var controller in controllers)
      {
        Initializer<Controller>.Initialize(controller);
      }

      IsReady = true;
    }
  }
}