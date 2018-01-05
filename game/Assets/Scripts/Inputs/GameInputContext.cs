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
        var value = Input.GetAxis(inputName);
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

    private IEnumerator ProcessUpdate()
    {
      while (true)
      {
        yield return null;
        ProcessEvents(InputUpdate.Update, _updateInputs);
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