using Injection;
using Utils;

namespace Game.Modules
{
  public interface IContext
  {
    IInjector Injector { get; }
    Lifetime Lifetime { get; }
  }
}
