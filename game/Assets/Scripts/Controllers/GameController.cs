using Injection;

namespace Game.Controllers
{
  public class GameController : Controller
  {
    [Inject]
    private GameContext _context;

    protected GameContext Context { get { return _context; } }
  }
}