using System.Collections.Generic;
using Game.Managers;

namespace Game.Providers
{
  public class GameManagersProvider
  {
    public IEnumerable<GameManager> Providers(GameContext context)
    {
      yield return new GamePreloaderManager();
      yield return new GameSceneManager();
      yield return new GamePersistanceManager();
      yield return new GameLevelManager();
      yield return new GameLevelLogicManager();
      yield return new GamePlayersManager();
      yield return new GameStateManager();
      yield return new GameSoundManager();
    }
  }
}