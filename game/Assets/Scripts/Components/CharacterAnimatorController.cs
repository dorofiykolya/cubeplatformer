using UnityEngine;

namespace Game.Components
{
  public class CharacterAnimatorController : MonoBehaviour
  {
    [SerializeField]
    private Animator _animator;

    private CharacterTrigger _trigger;

    private void Awake()
    {
      if (_animator == null) _animator = GetComponent<Animator>();
      if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    public CharacterTrigger Trigger
    {
      get { return _trigger; }
      set
      {
        if (_trigger != value)
        {
          var last = _trigger.ToString();
          _trigger = value;
          if (_animator != null)
          {
            _animator.ResetTrigger(last);
            var triggerName = _trigger.ToString();
            _animator.SetTrigger(triggerName);
            Debug.Log("Trigger-> " + triggerName);
          }
        }
      }
    }

    public void Play()
    {
      //_animator.enabled = false;
    }

    public void Stop()
    {
      //_animator.enabled = true;
    }
  }
}
