using UnityEngine;

namespace Game.Components
{
  public class CellGuardContentComponent : CellContentComponent
  {
    [SerializeField]
    public CharacterAnimatorController Controller;

    private void Awake()
    {
      if (Controller == null) Controller = GetComponent<CharacterAnimatorController>();
    }

    private void Reset()
    {
      if (Controller == null) Controller = GetComponent<CharacterAnimatorController>();
      if (Controller == null && GetComponent<Animator>() != null) Controller = gameObject.AddComponent<CharacterAnimatorController>();
    }

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

    public void HaGold(bool hasGold)
    {

    }
  }
}