using Game.Controllers;
using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(Animator))]
  public class AnimatorTimeComponent : MonoBehaviour
  {
    public TimeGroup TimeGroup;

    private Animator _animator;
    private float _startSpeed;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
      _startSpeed = _animator.speed;
    }

    public float Speed
    {
      get { return _animator.speed; }
      set { _animator.speed = value; }
    }

    private void Update()
    {
      var scale = LevelControllerComponent.Current.TimeController.GetTimeScale(TimeGroup);
      Speed = _startSpeed * scale;
    }
  }
}
