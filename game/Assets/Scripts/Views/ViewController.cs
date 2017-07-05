using Injection;
using Utils;

namespace Game.Views.Controllers
{
  public class ViewController
  {
    [Inject]
    private ViewContext _viewContext;
    [Inject]
    private GameContext _context;
    [Inject]
    private Lifetime _lifetime;

    internal void PreinitializeController()
    {
      _lifetime.AddAction(OnDispose);
      Preinitialize();
    }

    internal void InitializeController()
    {
      Initialize();
    }

    protected virtual void Preinitialize()
    {

    }

    protected virtual void Initialize()
    {

    }

    protected virtual void OnDispose()
    {

    }

    protected GameContext Context { get { return _context; } }
    protected ViewContext ViewContext { get { return _viewContext; } }
    protected Lifetime Lifetime { get { return _lifetime; } }
  }
}