using UnityEngine;
using Utils;

namespace Game.Components
{
  [RequireComponent(typeof(TimeControllerComponent))]
  public class LevelControllerComponent : MonoBehaviour
  {
    public static LevelControllerComponent Current;

    private Lifetime.Definition _instanceDefinition;
    private GameContext _gameContext;

    private void Awake()
    {
      if (_instanceDefinition == null) _instanceDefinition = Lifetime.Define(Lifetime.Eternal);
      Current = this;

      TimeController = GetComponent<TimeControllerComponent>();
    }

    private void OnDestroy()
    {
      _instanceDefinition.Terminate();
      if (Current == this) Current = null;
    }

    public TimeControllerComponent TimeController { get; private set; }

    public Lifetime Lifetime
    {
      get
      {
        if (_instanceDefinition == null) _instanceDefinition = Lifetime.Define(Lifetime.Eternal);
        return _instanceDefinition.Lifetime;
      }
    }

    public GameContext Context { get { return _gameContext; } }

    public void Load(GameContext context, Lifetime lifetime)
    {
      _gameContext = context;
      lifetime.AddAction(_instanceDefinition.Terminate);
    }
  }
}
