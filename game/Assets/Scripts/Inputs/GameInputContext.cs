using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Inputs
{
  public class GameInputContext : InputContext
  {
#if UNITY_EDITOR
    private static readonly HashSet<string> _axiesSet = new HashSet<string>();
#endif

    public static bool LogEvents = false;
    private readonly Dictionary<int, TouchInputEvent> _touches;
    private readonly Dictionary<string, InputEvent> _updateInputs;
    private readonly Dictionary<string, InputEvent> _fixedUpdateInputs;
    private readonly Dictionary<string, InputController> _controllers;
    private readonly GameContext _context;
    private readonly int MaxControllers = 4;

    public GameInputContext(GameContext context) : base(context, context.Lifetime)
    {
      _context = context;
      _updateInputs = new Dictionary<string, InputEvent>();
      _fixedUpdateInputs = new Dictionary<string, InputEvent>();
      _controllers = new Dictionary<string, InputController>(MaxControllers);
      _touches = new Dictionary<int, TouchInputEvent>();
      Input.simulateMouseWithTouches = true;

      context.StartCoroutine(context.Lifetime, CheckForControllers());
      context.StartCoroutine(context.Lifetime, ProcessUpdate());
      context.StartCoroutine(context.Lifetime, ProcessFixedUpdate());
    }

    public override InputController[] Controllers
    {
      get { return _controllers.Values.ToArray(); }
    }

    private IEnumerator CheckForControllers()
    {
      yield return null;

      while (true)
      {
        var toRemove = ListPool<string>.Pop(_controllers.Keys);
        var toAdd = ListPool<string>.Pop();

        var controllers = Input.GetJoystickNames().Where(c => !string.IsNullOrEmpty(c));

        foreach (var controller in controllers)
        {
          if (_controllers.ContainsKey(controller))
          {
            toRemove.Remove(controller);
          }
          else
          {
            toAdd.Add(controller);
          }
        }

        foreach (var c in toRemove)
        {
          var controller = _controllers[c];
          controller.Active = false;
          //_controllers.Remove(c);
          FireDeactiveController(controller);
        }

        foreach (var addId in toAdd)
        {
          var id = 0;
          for (int i = 0; i < MaxControllers; i++, id++)
          {
            if (_controllers.All(pair => pair.Value.Id != id))
            {
              break;
            }
            else
            {
              var cntrol = _controllers.Values.First(ct => ct.Id == i);
              if (!cntrol.Active)
              {
                cntrol.Active = true;
                FireActivateController(cntrol);
              }
            }
          }
          var controller = new InputController(addId, id, _context.Lifetime);
          _controllers.Add(addId, controller);
          FireAddController(controller);
          controller.Active = true;
          FireActivateController(controller);
        }

        ListPool.Push(toAdd);
        ListPool.Push(toRemove);

        yield return new WaitForSeconds(1f);
      }
    }

    private void ProcessEvents(InputUpdate update, Dictionary<string, InputEvent> inputs)
    {
#if UNITY_EDITOR
      for (int joyIndex = 1; joyIndex < 12; joyIndex++)
      {
        for (int axiesIndex = 1; axiesIndex < 30; axiesIndex++)
        {
          var name = string.Format("Joy{0}Axis{1}", joyIndex, axiesIndex);
          var value = Input.GetAxis(name);
          var contains = Math.Abs(value) > float.Epsilon;
          if (contains)
          {
            if (_axiesSet.Add(name))
            {
              Debug.Log("Input-Begin: " + name + ", " + update);
            }
          }
          else
          {
            if (_axiesSet.Contains(name))
            {
              Debug.Log("Input-End: " + name + ", " + update);
            }
            _axiesSet.Remove(name);
          }
        }
      }
#endif

      foreach (var input in InputNameMapper<GameInput>.Collection)
      {
        var inputName = input.Name;
        var parent = input.Parent;
        if (parent != null)
        {
          inputName = input.Parent.Name;
        }
        var value = input.Input == GameInput.InputValue.Input ? Input.GetAxis(inputName) : Input.GetAxisRaw(inputName);
        bool contains = false;
        if (parent != null)
        {
          if (input.Value == GameInput.ValueType.Hight)
          {
            contains = value > float.Epsilon;
          }
          else if (input.Value == GameInput.ValueType.Low)
          {
            contains = value < -float.Epsilon;
          }
          else
          {
            contains = Math.Abs(value) > float.Epsilon;
          }
        }
        else
        {
          contains = Math.Abs(value) > float.Epsilon;
        }

        InputEvent evt;
        if (inputs.TryGetValue(input.Name, out evt))
        {
          if (!contains)
          {
            FireEvent(new InputEvent
            {
              Input = input,
              Phase = InputPhase.End,
              Value = evt.Value,
              Update = update
            });
            if (LogEvents) _context.Logger.Log("Input - End: [" + input.Name + "] " + value + ", " + update);
            inputs.Remove(input.Name);
          }
          else
          {
            FireEvent(new InputEvent
            {
              Input = input,
              Phase = InputPhase.Process,
              Value = value,
              Update = update
            });
            if (LogEvents) _context.Logger.Log("Input - Process: [" + input.Name + "] " + value + ", " + update);
          }
        }
        else if (contains)
        {
          evt = new InputEvent
          {
            Input = input,
            Value = value,
            Phase = InputPhase.Begin,
            Update = update
          };
          inputs.Add(input.Name, evt);
          if (LogEvents) _context.Logger.Log("Input - Began: [" + input.Name + "] " + value + ", " + update);
          FireEvent(evt);
        }
      }
    }

    private void ProcessTouches()
    {
      if (Input.GetMouseButtonDown(0) && !_touches.ContainsKey(0))
      {
        var info = TouchInputEvent.Pool.Pop(0, Input.mousePosition);
        _touches.Add(0, info);
        FireEvent(info);
      }
      else if (Input.GetMouseButton(0) && _touches.ContainsKey(0))
      {
        _touches[0].Update(Input.mousePosition);
        FireEvent(_touches[0]);
      }
      else if (Input.GetMouseButtonUp(0) && _touches.ContainsKey(0))
      {
        _touches[0].End(Input.mousePosition);
        FireEvent(_touches[0]);
        _touches.Remove(0);
      }
      foreach (var touch in Input.touches)
      {
        if (touch.phase != TouchPhase.Stationary)
        {
          TouchInputEvent info;
          if (!_touches.TryGetValue(touch.fingerId + 1, out info))
          {
            if (touch.phase == TouchPhase.Began)
            {
              info = TouchInputEvent.Pool.Pop(touch.fingerId + 1, touch.position);
              _touches.Add(info.Id, info);
              FireEvent(info);
            }
          }
          else
          {
            if (touch.phase == TouchPhase.Moved)
            {
              info.Update(touch.position);
              FireEvent(info);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
              info.End(touch.position);
              FireEvent(info);
              _touches.Remove(info.Id);
              TouchInputEvent.Pool.Release(info);
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
              info.Cancel(touch.position);
              FireEvent(info);
              _touches.Remove(info.Id);
              TouchInputEvent.Pool.Release(info);
            }
          }
        }
      }
    }

    private IEnumerator ProcessUpdate()
    {
      while (true)
      {
        yield return null;
        ProcessEvents(InputUpdate.Update, _updateInputs);
        ProcessTouches();
      }
    }

    private IEnumerator ProcessFixedUpdate()
    {
      while (true)
      {
        yield return new WaitForFixedUpdate();
        ProcessEvents(InputUpdate.FixedUpdate, _fixedUpdateInputs);
      }
    }
  }
}