using Injection;

namespace Game.Managers
{
  public class GameLevelLogicManager : GameManager
  {
    [Inject]
    private GameLevelManager _levelManager;

    protected override void Initialize()
    {
      _levelManager.SubscribeOnLoaded(Lifetime, LevelLoadedHandler);
      _levelManager.SubscribeOnUnloaded(Lifetime, LevelUnloadedHandler);
    }

    private void LevelUnloadedHandler(GameClassicLevelInfo info)
    {
      
    }

    private void LevelLoadedHandler(GameClassicLevelInfo info)
    {
      var logic = info.Level.Logic;
      //info.Level.CoordinateConverter
    }
  }
}