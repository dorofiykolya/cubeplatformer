using UnityEngine;

namespace Game.Components
{
  public class CubeRandomRotateComponent : MonoBehaviour
  {
    [SerializeField]
    private float _time = 1f;
    [SerializeField]
    private float _speed = 1f;
    private Quaternion _target;
    private float _passedTime = 0f;

    private void Update()
    {
      _passedTime += Time.deltaTime * _speed;
      int count = (int)(_passedTime / _time);
      if (count > 0)
      {
        _passedTime -= _time * count;
        _target = Random.rotation;
      }
      transform.rotation = Quaternion.Lerp(transform.rotation, _target, _passedTime / _time);
    }
  }
}