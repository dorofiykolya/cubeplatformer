using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Inputs
{
  public class GameInputContenxt : InputContext
  {
    private readonly HashSet<string> _inputs;
    private readonly Dictionary<string, InputController> _controllers;

    public GameInputContenxt(GameContext context) : base(context, context.Lifetime)
    {
      _inputs = new HashSet<string>();
      _controllers = new Dictionary<string, InputController>();

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

        var controllers = Input.GetJoystickNames();

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
          _controllers.Remove(c);
          FireRemoveController(controller);
        }

        foreach (var c in toAdd)
        {
          var id = 0;
          for (int i = 0; i < 4; i++, id++)
          {
            if (!_controllers.Any(pair => pair.Value.Id == id))
            {
              break;
            }
          }
          var controller = new InputController(c, id);
          _controllers.Add(c, controller);
          FireAddController(controller);
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
        foreach (var input in GameInputField.Inputs)
        {
          var value = Input.GetAxis(input.Name);
          var contains = Math.Abs(value) > float.Epsilon;

          if (_inputs.Contains(input.Name))
          {
            if (!contains)
            {
              Fire(new InputEvent
              {
                Input = input.Input,
                Phase = InputPhase.End,
                Value = value
              });

              _inputs.Remove(input.Name);
            }
            else
            {
              Fire(new InputEvent
              {
                Input = input.Input,
                Phase = InputPhase.Process,
                Value = value
              });
            }
          }
          else if (contains)
          {
            _inputs.Add(input.Name);
            Fire(new InputEvent
            {
              Input = input.Input,
              Value = value,
              Phase = InputPhase.Begin
            });
          }
        }
        yield return null;
      }
    }
  }
}