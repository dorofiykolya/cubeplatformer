using UnityEngine;
using Game.Components;

namespace Game.Providers
{
  public class PreloaderProvider
  {
    public GamePreloaderComponent GetPrefab()
    {
      return Resources.Load<GamePreloaderComponent>("Preloader");
    }
  }
}