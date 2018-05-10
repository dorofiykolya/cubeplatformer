using Game.Components;
using Utils;

namespace Game.Controllers
{
  public class CurrentLevelInfo
  {
    private readonly int _index;
    private readonly int _subLevel;
    private readonly Lifetime.Definition _lifetime;

    public CurrentLevelInfo(Lifetime lifetime)
    {
      _lifetime = Lifetime.Define(lifetime);
    }

    public CurrentLevelInfo(Lifetime lifetime, GameClassicLevelInfo classicLevel, int index, int subLevel) : this(lifetime)
    {
      _index = index;
      _subLevel = subLevel;
      ClassicLevel = classicLevel;
    }

    public CurrentLevelInfo(Lifetime lifetime, LevelControllerComponent levelController, int index, int subLevel) : this(lifetime)
    {
      _index = index;
      _subLevel = subLevel;
      LevelController = levelController;
    }

    public LevelControllerComponent LevelController { get; private set; }
    public GameClassicLevelInfo ClassicLevel { get; private set; }
    public Lifetime Lifetime { get { return _lifetime.Lifetime; } }
    public int Level { get { return _index; } }
    public int SubLevel { get { return _subLevel; } }

    public void Terminate()
    {
      _lifetime.Terminate();
    }
  }
}
