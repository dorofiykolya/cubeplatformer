using System;
using Utils;

namespace Game
{
  public class TimeManager
  {
    private readonly GameStartBehaviour _behaviour;

    public TimeManager(GameStartBehaviour behaviour)
    {
      _behaviour = behaviour;
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
  }
}