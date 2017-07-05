using Injection;

namespace Game.UI.Controllers
{
  public class UIController : Controller
  {
    [Inject]
    private GameContext _context;
    [Inject]
    private UIContext _uiContext;

    protected GameContext Context { get { return _context; } }
    protected UIContext UIContext { get { return _uiContext; } }
  }
}