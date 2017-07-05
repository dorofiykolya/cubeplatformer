using Injection;
using Utils;

namespace Game.Managers
{
  public class GameManager
  {
    [Inject]
    private GameContext _context;
    [Inject]
    private Lifetime _lifetime;

    internal void InitializeManager()
    {
      Initialize();
    }

    internal void PreinitiliazeManager()
    {
      _lifetime.AddAction(OnDispose);
      Preinitialize();
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void Preinitialize()
    {

    }

    protected virtual void OnDispose()
    {

    }

    protected Lifetime Lifetime { get { return _lifetime; } }
    protected GameContext Context { get { return _context; } }
  }
}