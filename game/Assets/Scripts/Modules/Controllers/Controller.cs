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
      OnPreinitialize();
    }

    [Initialize]
    private void InitializeController()
    {
      OnInitialize();
    }

    protected Lifetime Lifetime
    {
      get { return _lifetime; }
    }

    protected virtual void OnPreinitialize()
    {

    }

    protected virtual void OnInitialize()
    {

    }

    protected virtual void OnDispose()
    {

    }
  }
}