using Injection;
using Utils;

namespace Game
{
  public class Controller
  {
    [Inject]
    private Lifetime _lifetime;

    [Preinitialize]
    private void PreinitializeController()
    {
      _lifetime.AddAction(OnDispose);
      Preinitialize();
    }

    [Initialize]
    private void InitializeController()
    {
      Initialize();
    }

    protected Lifetime Lifetime
    {
      get { return _lifetime; }
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
  }
}