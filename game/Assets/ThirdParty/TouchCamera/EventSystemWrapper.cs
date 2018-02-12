using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine
{
  [RequireComponent(typeof(EventSystem))]
  public class EventSystemWrapper : MonoBehaviour
  {
    private static readonly HashSet<string> _hashSet = new HashSet<string>();
    private static EventSystem _current;

    public static EventSystem current
    {
      get { return _current; }
      set
      {
        if (value != null)
        { _current = value; }
      }
    }

    public static bool IsCurrentEnabled
    {
      get { return current != null && current.enabled; }
    }

    public static bool IsPointerOverGameObject()
    {
      return current != null && (current.IsPointerOverGameObject() || !current.enabled);
    }

    public static bool CanDoubleClick()
    {
      return true;
    }

    public static bool CanDrag()
    {
      return true;
    }

    public static bool CanPinch()
    {
      return true;
    }


    public static bool IsPointerOverGameObject(int pointerId)
    {
      return current != null && (current.IsPointerOverGameObject(pointerId) || !current.enabled);
    }

    public static void SetEnable(string id, bool value)
    {
      if (value)
      {
        _hashSet.Remove(id);
      }
      else
      {
        _hashSet.Add(id);
      }
      current.enabled = _hashSet.Count == 0;
    }

    public static void SetEnable()
    {
      _hashSet.Clear();
      current.enabled = _hashSet.Count == 0;
    }

    [SerializeField]
    private EventSystem _eventSystem;
    [SerializeField]
    private bool _doNotDestroyEventSystem;

    private void Awake()
    {
      if (_eventSystem == null)
      {
        _eventSystem = GetComponent<EventSystem>();
      }
      current = _eventSystem;
      if (current != null)
      {
        DontDestroyOnLoad(current);
      }
    }
  }
}
