using System;
using BitBenderGames;
using UnityEngine;

namespace Game.Components.MenuNavigation
{
  public class MenuNavigationTouchCameraItemSelectorComponent : MonoBehaviour
  {
    public MenuNavigationComponent Navigation;
    public MobileTouchCamera TouchCamera;
    public TouchInputController TouchInputController;

    private void Awake()
    {
      TouchInputController.OnInputClick += TouchInputControllerOnOnInputClick;
    }

    private void TouchInputControllerOnOnInputClick(Vector3 clickPosition, bool isDoubleClick, bool isLongTap)
    {
      var position = TouchCamera.Cam.ScreenToWorldPoint(clickPosition);
      var overlapCollider = Physics2D.OverlapPoint(position);
      NavigationComponent navigationComponent;
      if (overlapCollider != null && (navigationComponent = overlapCollider.GetComponent<NavigationComponent>()) != null)
      {
        Navigation.Select(navigationComponent);
        Navigation.FireAction();
      }
    }

    private void Update()
    {
      if (TouchCamera.IsDragging || TouchCamera.IsAutoScrolling || TouchCamera.IsMoveTo)
      {
        var overlapCollider = Physics2D.OverlapPoint(transform.position);
        NavigationComponent navigationComponent;
        if (overlapCollider != null && (navigationComponent = overlapCollider.GetComponent<NavigationComponent>()) != null)
        {
          Navigation.Select(navigationComponent);
        }
      }
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;

      Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * 50));
    }
  }
}
