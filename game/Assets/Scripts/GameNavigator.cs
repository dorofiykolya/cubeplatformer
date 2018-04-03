using Game.Messages;

namespace Game
{
  public class GameNavigator
  {
    private readonly GameContext _gameContext;

    public GameNavigator(GameContext gameContext)
    {
      _gameContext = gameContext;
    }

    public void OpenMainMenu()
    {
      _gameContext.Tell(new OpenMainMenuMessage());
    }

    public void OpenSubLevel()
    {
      _gameContext.Tell(new OpenSubLevelMenuMessage());
    }
  }
}
