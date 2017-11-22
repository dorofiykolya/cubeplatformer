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

    private readonly Dictionary<string, InputEvent> _inputs;
    private readonly Dictionary<string, InputController> _controllers;
    private readonly GameContext _context;
    private readonly int MaxControllers = 4;

    public GameInputContext(GameContext context) : base(context, context.Lifetime)
    {
      _context = context;
      _inputs = new Dictionary<string, InputEvent>();
      _controllers = new Dictionary<string, InputController>(MaxControllers);

      context.StartCoroutine(context.Lifetime, CheckForControllers());
      context.StartCoroutine(context.Lifetime, Process());
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

    private IEnumerator Process()
    {
      yield return null;

      while (true)
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
                Debug.Log("Input-Begin: " + name);
              }
            }
            else
            {
              if (_axiesSet.Contains(name))
              {
                Debug.Log("Input-End: " + name);
              }
              _axiesSet.Remove(name);
            }
          }
        }
#endif

        foreach (var input in InputNameMapper<GameInput>.Collection)
        {
          var value = Input.GetAxis(input.Name);
          var contains = Math.Abs(value) > float.Epsilon;

          InputEvent evt;
          if (_inputs.TryGetValue(input.Name, out evt))
          {
            if (!contains)
            {
              FireEvent(new InputEvent
              {
                Input = input,
                Phase = InputPhase.End,
                Value = evt.Value
              });
              if (LogEvents) _context.Logger.Log("Input - End: [" + input.Name + "] " + value);
              _inputs.Remove(input.Name);
            }
            else
            {
              FireEvent(new InputEvent
              {
                Input = input,
                Phase = InputPhase.Process,
                Value = value
              });
              if (LogEvents) _context.Logger.Log("Input - Process: [" + input.Name + "] " + value);
            }
          }
          else if (contains)
          {
            evt = new InputEvent
            {
              Input = input,
              Value = value,
              Phase = InputPhase.Begin
            };
            _inputs.Add(input.Name, evt);
            if (LogEvents) _context.Logger.Log("Input - Began: [" + input.Name + "] " + value);
            FireEvent(evt);
          }
        }
        yield return null;
      }
    }
  }
}