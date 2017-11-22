using UnityEngine;

namespace Game.Components
{
  public class RotateComponent : MonoBehaviour
  {
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _speed;

    private void Awake()
    {
      if (_target == null) _target = transform;
    }

    private void Update()
    {
      if (_target != null)
      {
        _target.Rotate(_speed * Time.deltaTime);
      }
    }
  }
}
