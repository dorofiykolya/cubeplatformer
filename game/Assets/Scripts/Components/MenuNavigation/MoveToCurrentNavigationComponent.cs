using UnityEngine;

namespace Game.Components.MenuNavigation
{
  public class MoveToCurrentNavigationComponent : MonoBehaviour
  {
    public MenuNavigationComponent Navigation;
    public Transform Target;
    [Tooltip("seconds")]
    public float NavigationTime = 1f;
    public AnimationCurve Curve;

    public bool FreezeX;
    public bool FreezeY;
    public bool FreezeZ;

    private NavigationComponent _navigationComponent;
    private float _passedTime;
    private Vector3 _prevPosition;

    private void Awake()
    {
      if (Navigation == null)
      {
        Navigation = GetComponent<MenuNavigationComponent>();
        if (Navigation == null) Navigation = GetComponentInParent<MenuNavigationComponent>();
      }
    }

    private void Update()
    {
      if (Target != null && Navigation != null && Navigation.Current != null)
      {
        if (_navigationComponent != Navigation.Current)
        {
          _navigationComponent = Navigation.Current;
          _prevPosition = Target.position;
          _passedTime = 0;
        }

        _passedTime += Time.deltaTime;

        var ratio = Mathf.Clamp01(_passedTime / NavigationTime);
        var value = Curve.Evaluate(ratio);

        var position = Target.position;
        var current = Navigation.Current;

        var currentTransform = current.CameraPivotTransform != null ? current.CameraPivotTransform : current.Target;

        var curPos = currentTransform.position;

        var delta = curPos - _prevPosition;

        var newPosition = _prevPosition + new Vector3(delta.x * value, delta.y * value, delta.z * value);

        if (FreezeX) newPosition.x = position.x;
        if (FreezeY) newPosition.y = position.y;
        if (FreezeZ) newPosition.z = position.z;
        Target.position = newPosition;
      }
    }
  }
}
