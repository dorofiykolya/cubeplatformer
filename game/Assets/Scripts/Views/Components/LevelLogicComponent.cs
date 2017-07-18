using Game.Logics;
using UnityEngine;

namespace Game.Views.Components
{
  [RequireComponent(typeof(LevelComponent))]
  public class LevelLogicComponent : MonoBehaviour
  {
    public virtual ILogicEngine Engine
    {
      get { return (GetComponent<ClassicLevelLogicComponent>() ?? gameObject.AddComponent<ClassicLevelLogicComponent>()).Engine; }
    }
  }
}