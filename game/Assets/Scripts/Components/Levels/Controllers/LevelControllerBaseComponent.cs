using UnityEngine;
using Utils;

namespace Game.Components
{
  public class LevelControllerBaseComponent : MonoBehaviour
  {
    private LevelControllerComponent _levelController;

    public LevelControllerComponent LevelController { get { return _levelController; } }
    public GameContext Context { get { return _levelController.Context; } }
    public Lifetime Lifetime { get { return _levelController.Lifetime; } }

    private void Awake()
    {
      _levelController = GetComponent<LevelControllerComponent>();
      OnAwake();
    }

    protected virtual void OnAwake()
    {

    }
  }
}
