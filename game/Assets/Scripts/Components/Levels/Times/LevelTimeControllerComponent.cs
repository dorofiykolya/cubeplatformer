using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils;

namespace Game.Components
{
  [RequireComponent(typeof(LevelControllerComponent))]
  public class LevelTimeControllerComponent : LevelControllerBaseComponent
  {
    [Range(0f, 5f)]
    public float TimeScale = 1f;

    private Signal<float> _onUpdate;


    protected override void OnAwake()
    {
      _onUpdate = new Signal<float>(Lifetime);
    }

    public void SubscribeOnUpdate(Lifetime lifetime, Action<float> listener)
    {
      var deltaTime = TimeScale * Context.Time.DeltaTime;
      _onUpdate.Fire(deltaTime);
    }

    public float GetTimeScale(TimeGroup timeGroup)
    {
      return TimeScale;
    }
  }
}
