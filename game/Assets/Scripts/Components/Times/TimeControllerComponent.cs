using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils;

namespace Game.Components
{
  [RequireComponent(typeof(LevelControllerComponent))]
  public class TimeControllerComponent : MonoBehaviour
  {
    [Range(0f, 5f)]
    public float TimeScale = 1f;

    private LevelControllerComponent _levelController;
    private Signal<float> _onUpdate;

    private void Awake()
    {
      _levelController = GetComponent<LevelControllerComponent>();
      _onUpdate = new Signal<float>(_levelController.Lifetime);
    }

    public void SubscribeOnUpdate(Lifetime lifetime, Action<float> listener)
    {
      var deltaTime = TimeScale * _levelController.Context.Time.DeltaTime;
      _onUpdate.Fire(deltaTime);
    }

    public float GetTimeScale(TimeGroup timeGroup)
    {
      return TimeScale;
    }
  }
}
