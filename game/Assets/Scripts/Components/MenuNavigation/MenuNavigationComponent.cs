using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utils;

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

    [Header("Events")]
    public UnityEvent OnGoTo;

    private Lifetime.Definition _lifetimeDefinition;
    private Signal _onAction;

    private void Awake()
    {
      _lifetimeDefinition = Lifetime.Define(Lifetime.Eternal);
      _onAction = new Signal(_lifetimeDefinition.Lifetime);
      if (Current == null)
      {
        Current = Target;
      }
    }

    private void OnDestory()
    {
      _lifetimeDefinition.Terminate();
    }

    private void DispatchEvent()
    {
      if (Previous)
      {
        Previous.OnUnselected.Invoke();
      }

      if (Current)
      {
        Current.OnSelected.Invoke();
      }
    }

    public void SubscribeOnAction(Lifetime lifetime, Action listener)
    {
      _onAction.Subscribe(lifetime, listener);
    }

    public void FireAction()
    {
      _onAction.Fire();
    }

    public void Select(NavigationComponent component)
    {
      if (component == null)
      {
        if (Current != null)
        {
          Previous = Current;
          Current = Target;
        }
      }
      else
      {
        if (Current != component)
        {
          Previous = Current;
          Current = component;
        }
      }

      DispatchEvent();
    }

    public void UnSelect(NavigationComponent component)
    {
      if (component != null)
      {
        if (Current == component)
        {
          Select(null);
        }
      }
    }

    public void GoTo(NavigationComponent component)
    {
      Select(component);
      if (OnGoTo != null) OnGoTo.Invoke();
    }

    public void GoToBack()
    {
      if (Previous != null)
      {
        GoTo(Previous);
      }
    }

    public void GoToLeft()
    {
      if (Current.Left != null)
      {
        GoTo(Current.Left);
      }
    }

    public void GoToRight()
    {
      if (Current.Right != null)
      {
        GoTo(Current.Right);
      }
    }

    public void GoToTop()
    {
      if (Current.Top != null)
      {
        GoTo(Current.Top);
      }
    }

    public void GoToBottom()
    {
      if (Current.Bottom != null)
      {
        GoTo(Current.Bottom);
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
