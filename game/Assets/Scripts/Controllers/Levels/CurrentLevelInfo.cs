using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Components;
using Utils;

namespace Game.Controllers
{
  public class CurrentLevelInfo
  {
    private readonly Lifetime.Definition _lifetime;

    public CurrentLevelInfo(Lifetime lifetime)
    {
      _lifetime = Lifetime.Define(lifetime);
    }

    public CurrentLevelInfo(Lifetime lifetime, GameClassicLevelInfo classicLevel) : this(lifetime)
    {
      ClassicLevel = classicLevel;
    }

    public CurrentLevelInfo(Lifetime lifetime, LevelControllerComponent levelController) : this(lifetime)
    {
      LevelController = levelController;
    }

    public LevelControllerComponent LevelController { get; private set; }
    public GameClassicLevelInfo ClassicLevel { get; private set; }
    public Lifetime Lifetime { get { return _lifetime.Lifetime; } }

    public void Terminate()
    {
      _lifetime.Terminate();
    }
  }
}
