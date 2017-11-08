using Utils;

namespace Game.Providers
{
  public class GameProviders
  {
    private readonly Lifetime _lifetime;
    private readonly GameStartBehaviour _behaviour;

    public readonly PreloaderProvider Preloader;
    public readonly ICoroutineProvider CoroutineProvider;
    public readonly ResourcePrefabPathProvider ResourcePrefabPathProvider;

    public GameProviders(Lifetime lifetime, GameStartBehaviour behaviour)
    {
      _lifetime = lifetime;
      _behaviour = behaviour;

      Preloader = new PreloaderProvider();
      CoroutineProvider = new CoroutineProvider(behaviour);
      ResourcePrefabPathProvider = new ResourcePrefabPathProvider();
    }
  }
}