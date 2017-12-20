using UnityEngine;
using Utils;

namespace Game.Components
{
  public class LevelControllerComponent : MonoBehaviour
  {
    private Lifetime.Definition _instanceDefinition;
    private Lifetime.Definition _lifetimeDefinition;
    private GameContext _gameContext;

    private void Awake()
    {
      _instanceDefinition = Lifetime.Define(Lifetime.Eternal);
    }

    private void OnDestroy()
    {
      _instanceDefinition.Terminate();
    }

    public Lifetime Lifetime { get { return _lifetimeDefinition.Lifetime; } }
    public GameContext Context { get { return _gameContext; } }

    public void Load(GameContext context, Lifetime lifetime)
    {
      if (_lifetimeDefinition != null) _lifetimeDefinition.Terminate();
      _lifetimeDefinition = Lifetime.Intersection(_instanceDefinition.Lifetime, lifetime);
      _gameContext = context;
    }
  }
}
