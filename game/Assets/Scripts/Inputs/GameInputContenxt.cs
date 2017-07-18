using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Inputs
{
  public class GameInputContenxt : InputContext
  {
    private readonly HashSet<string> _inputs;
    private int _controllers;

    public GameInputContenxt(GameContext context) : base(context, context.Lifetime)
    {
      _inputs = new HashSet<string>();

      context.StartCoroutine(context.Lifetime, CheckForControllers());
      context.StartCoroutine(context.Lifetime, Process());
    }

    public override int Controllers
    {
      get { return _controllers; }
    }

    private IEnumerator CheckForControllers()
    {
      while (true)
      {
        var controllers = Input.GetJoystickNames();
        if (controllers.Length != _controllers)
        {
          _controllers = controllers.Length;
          FireControllersChanged();
        }
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
                Status = InputStatus.End,
                Value = value
              });

              _inputs.Remove(input.Name);
            }
            else
            {
              Fire(new InputEvent
              {
                Input = input.Input,
                Status = InputStatus.Process,
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
              Status = InputStatus.Begin
            });
          }
        }
        yield return null;
      }
    }
  }
}