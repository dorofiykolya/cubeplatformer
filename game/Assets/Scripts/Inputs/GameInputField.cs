using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using Game.Inputs;

namespace Game
{
  public class GameInputField
  {
    public GameInput Input { get; private set; }
    public string Name { get; private set; }

    private static GameInputField[] _staticFields;

    public static GameInputField[] Inputs
    {
      get
      {
        if (_staticFields == null)
        {
          var fields = typeof(GameInput).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
          var index = 0;
          _staticFields = new GameInputField[fields.Length];
          foreach (var fieldInfo in fields)
          {
            var value = fieldInfo.GetValue(typeof(GameInput)) as GameInput;
            if (value != null)
            {
              _staticFields[index] = new GameInputField
              {
                Input = value,
                Name = fieldInfo.Name
              };
              index++;
            }
          }
          Array.Resize(ref _staticFields, index);
        }
        return _staticFields;
      }
    }
  }
}