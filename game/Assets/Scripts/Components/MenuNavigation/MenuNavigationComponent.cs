using UnityEngine;

namespace Game.Components.MenuNavigation
{
  [ExecuteInEditMode]
  public class MenuNavigationComponent : MonoBehaviour
  {
    [Header("State")]
    public NavigationComponent Current;
    public NavigationComponent Previous;

    [Header("Target")]
    public NavigationComponent Target;

    private void Awake()
    {
      if (Current == null)
      {
        Current = Target;
      }
    }

    private void DispatchEvent()
    {
      Previous.OnUnselected.Invoke();
      Current.OnSelected.Invoke();
    }

    public void GoToBack()
    {
      if (Previous != null)
      {
        var current = Current;
        Current = Previous;
        Previous = current;
        DispatchEvent();
      }
    }

    public void GoToLeft()
    {
      if (Current.Left != null)
      {
        Previous = Current;
        Current = Current.Left;
        DispatchEvent();
      }
    }

    public void GoToRight()
    {
      if (Current.Right != null)
      {
        Previous = Current;
        Current = Current.Right;
        DispatchEvent();
      }
    }

    public void GoToTop()
    {
      if (Current.Top != null)
      {
        Previous = Current;
        Current = Current.Top;
        DispatchEvent();
      }
    }

    public void GoToBottom()
    {
      if (Current.Bottom != null)
      {
        Previous = Current;
        Current = Current.Bottom;
        DispatchEvent();
      }
    }

    private void OnDrawGizmos()
    {
      if (Current == null) return;

      float radius = 0.5f;

      var current = Current;
      var next = current.Left;
      while (next != null)
      {
        Gizmos.DrawWireSphere(current.transform.position, radius);
        Gizmos.DrawWireSphere(next.transform.position, radius);
        Gizmos.DrawLine(current.transform.position, next.transform.position);
        current = next;
        next = current.Left;
      }

      current = Current;
      next = current.Right;
      while (next != null)
      {
        Gizmos.DrawWireSphere(current.transform.position, radius);
        Gizmos.DrawWireSphere(next.transform.position, radius);
        Gizmos.DrawLine(current.transform.position, next.transform.position);
        current = next;
        next = current.Right;
      }

      current = Current;
      next = current.Top;
      while (next != null)
      {
        Gizmos.DrawWireSphere(current.transform.position, radius);
        Gizmos.DrawWireSphere(next.transform.position, radius);
        Gizmos.DrawLine(current.transform.position, next.transform.position);
        current = next;
        next = current.Top;
      }

      current = Current;
      next = current.Bottom;
      while (next != null)
      {
        Gizmos.DrawWireSphere(current.transform.position, radius);
        Gizmos.DrawWireSphere(next.transform.position, radius);
        Gizmos.DrawLine(current.transform.position, next.transform.position);
        current = next;
        next = current.Bottom;
      }

      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(Current.transform.position, radius);
    }
  }
}
