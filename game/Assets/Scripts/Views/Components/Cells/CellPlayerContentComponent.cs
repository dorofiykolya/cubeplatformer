using UnityEngine;
using System.Collections;
using Game.Components;
using CharacterController = Game.Components.CharacterController;

namespace Game.Views.Components
{
  public class CellPlayerContentComponent : CellContentComponent
  {
    [SerializeField]
    public CharacterController Controller;

    public void SetTrigger(CharacterTrigger trigger)
    {
      Controller.Trigger = trigger;
      Controller.Play();
    }

    public CharacterTrigger Trigger { get { return Controller.Trigger; } }

    public void Stop()
    {
      Controller.Stop();
    }
  }
}