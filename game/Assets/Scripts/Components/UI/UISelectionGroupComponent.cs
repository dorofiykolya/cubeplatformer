using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components
{
  public class UISelectionGroupComponent : MonoBehaviour
  {
    private static readonly LinkedList<UISelectionGroupComponent> _stack = new LinkedList<UISelectionGroupComponent>();

    public Transform Content;
    public Selectable Default;

    private bool _entered;
    private Selectable[] _selectables;

    protected void Enter()
    {
      if (!_entered)
      {
        _entered = true;
        if (_selectables != null)
        {
          foreach (var selectable in _selectables)
          {
            selectable.interactable = true;
          }
        }

        if (Default)
        {
          Default.Select();
        }
      }
    }

    protected void Exit()
    {
      if (_entered)
      {
        _entered = false;
        _selectables = Content.GetComponentsInChildren<Selectable>();
        foreach (var selectable in _selectables)
        {
          selectable.interactable = false;
        }
      }
    }

    protected virtual void Awake()
    {
      Content = Content ?? transform;
    }

    protected virtual void OnEnable()
    {
      _stack.AddLast(this);
      ProcessStack();
      Enter();
    }

    protected virtual void OnDisable()
    {
      Exit();
      var last = _stack.Last.Value;
      _stack.Remove(this);
      if (last == this)
      {
        ProcessStack();
      }

      if (_stack.Count != 0)
      {
        _stack.Last.Value.Enter();
      }
    }

    private void ProcessStack()
    {
      foreach (var component in _stack)
      {
        if (component != _stack.Last.Value)
        {
          component.Exit();
        }
      }
    }
  }
}
