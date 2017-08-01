using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Game
{
  public class TimeManager
  {
    private readonly GameStartBehaviour _behaviour;
    private readonly LinkedList<Delay> _delay;

    public TimeManager(Lifetime lifetime, GameStartBehaviour behaviour)
    {
      _delay = new LinkedList<Delay>();
      _behaviour = behaviour;
      _behaviour.OnUpdate.Subscribe(lifetime, UpdateHandler);
    }

    public float DeltaTime { get { return Time.deltaTime; } }

    public void DelayCall(Lifetime lifetime, float delaySeconds, Action listener)
    {
      if (delaySeconds <= 0) throw new ArgumentException("delaySeconds must be > 0");

      var time = Time.time + delaySeconds;
      var first = _delay.First;
      var newDelay = new Delay
      {
        Time = time,
        Action = listener
      };
      while (first != null)
      {
        if (time < first.Value.Time)
        {
          lifetime.AddAction(() => _delay.Remove(newDelay));
          _delay.AddBefore(first, newDelay);
          return;
        }
        first = first.Next;
      }
      lifetime.AddAction(() => _delay.Remove(newDelay));
      _delay.AddLast(newDelay);
    }

    public void SubscribeOnUpdate(Lifetime lifetime, Action listener)
    {
      _behaviour.OnUpdate.Subscribe(lifetime, listener);
    }

    public void SubscribeOnLateUpdate(Lifetime lifetime, Action listener)
    {
      _behaviour.OnLateUpdate.Subscribe(lifetime, listener);
    }

    public void SubscribeOnFixedUpdate(Lifetime lifetime, Action listener)
    {
      _behaviour.OnFixedUpdate.Subscribe(lifetime, listener);
    }

    private void UpdateHandler()
    {
      if (_delay.Count != 0)
      {
        var time = Time.time;
        var delay = _delay.First;
        while (delay != null && time >= delay.Value.Time)
        {
          delay.Value.Action();
          _delay.RemoveFirst();
          delay = _delay.First;
        }
      }
    }

    private struct Delay
    {
      public float Time;
      public Action Action;
    }
  }
}