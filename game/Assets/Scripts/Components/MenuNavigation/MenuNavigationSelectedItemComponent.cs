using UnityEngine;

namespace Game.Components.MenuNavigation
{
  public class MenuNavigationSelectedItemComponent : MonoBehaviour
  {
    public Transform Target;
    public MenuNavigationComponent Navigation;
    public float MoveSpeed = 50f;
    public float ScaleSpeed = 50f;
    public float RotationSpeed = 360f;
    [Header("Arrows")]
    public Transform ArrowLeft;
    public Transform ArrowRight;
    public Transform ArrowUp;
    public Transform ArrowDown;

    private void Awake()
    {
      if (Navigation == null)
      {
        Navigation = GetComponent<MenuNavigationComponent>();
        if (Navigation == null) Navigation = GetComponentInParent<MenuNavigationComponent>();
      }
      if (Navigation != null && Navigation.Current != null)
      {
        var current = Navigation.Current.SelectedTransform;
        if (current != null)
        {
          Target.position = current.position;
          //Target.localScale = current.localScale;
          Target.rotation = current.rotation;
        }
      }
    }

    private void Update()
    {
      if (Target != null && Navigation != null && Navigation.Current != null)
      {
        var current = Navigation.Current.SelectedTransform;
        if (current != null)
        {
          if (ArrowLeft != null) ArrowLeft.gameObject.SetActive(Navigation.Current.Left != null);
          if (ArrowRight != null) ArrowRight.gameObject.SetActive(Navigation.Current.Right != null);
          if (ArrowDown != null) ArrowDown.gameObject.SetActive(Navigation.Current.Bottom != null);
          if (ArrowUp != null) ArrowUp.gameObject.SetActive(Navigation.Current.Top != null);

          Target.position = Vector3.MoveTowards(Target.position, current.position, MoveSpeed * Time.deltaTime);
          //Target.localScale = Vector3.MoveTowards(Target.localScale, current.localScale, ScaleSpeed * Time.deltaTime);
          Target.rotation = Quaternion.RotateTowards(Target.rotation, current.rotation, RotationSpeed * Time.deltaTime);
        }
      }
    }
  }
}
