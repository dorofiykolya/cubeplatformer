using Injection;

namespace Game.Controllers
{
  public class UserAbilitiesController : GameController
  {
    public const string JumpKey = "jump";
    public const string DiggKey = "digg";

    [Inject]
    private GamePersistanceController _persistance;

    protected override void OnPreinitialize()
    {

    }

    public bool Digg { get { return _persistance.GetInt(DiggKey, 0) == 1; } set { _persistance.SetInt(DiggKey, value ? 1 : 0); } }
    public bool Jump { get { return _persistance.GetInt(JumpKey, 0) == 1; } set { _persistance.SetInt(JumpKey, value ? 1 : 0); } }
  }
}
