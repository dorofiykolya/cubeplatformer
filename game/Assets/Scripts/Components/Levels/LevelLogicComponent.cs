using Game.Logics;
using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(LevelComponent))]
  public class LevelLogicComponent : MonoBehaviour
  {
    public virtual ILogicEngine Engine(GameContext context)
    {
      return (GetComponent<ClassicLevelLogicComponent>() ?? gameObject.AddComponent<ClassicLevelLogicComponent>()).Engine(context);
    }
  }
}