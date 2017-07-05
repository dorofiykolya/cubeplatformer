using Injection;

namespace Game.Managers
{
  public class GameManager : Controller
  {
    [Inject]
    private GameContext _context;

    protected GameContext Context { get { return _context; } }
  }
}