using System.Collections.Generic;
using Game.Controllers;

namespace Game.Providers
{
  public class GameControllersProvider
  {
    public IEnumerable<GameController> Providers(GameContext context)
    {
      yield return new GamePreloaderController();
      yield return new GameSceneController();
      yield return new GamePersistanceController();
      yield return new GameLevelController();
      yield return new GameLevelLogicController();
      yield return new GamePlayersController();
      yield return new GameStateController();
      yield return new GameSoundController();

      yield return new UserController();
      yield return new UserAbilitiesController();
      yield return new UserSkillsController();
    }
  }
}