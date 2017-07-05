using Injection;

namespace Game.Views.Controllers
{
  public class ViewController : Controller
  {
    [Inject]
    private ViewContext _viewContext;
    [Inject]
    private GameContext _context;

    protected GameContext Context { get { return _context; } }
    protected ViewContext ViewContext { get { return _viewContext; } }
  }
}