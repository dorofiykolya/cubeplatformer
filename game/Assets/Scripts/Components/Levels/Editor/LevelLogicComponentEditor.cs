using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Components;
using UnityEditor;

namespace Game.Editor
{
  [CustomEditor(typeof(LevelLogicComponent), true)]
  public class LevelLogicComponentEditor : Editor<LevelLogicComponent>
  {
    private static Type[] _types;

    public override void OnInspectorGUI()
    {
      if (_types == null)
      {
        var list = new List<Type>();
        foreach (var type in typeof(LevelLogicComponent).Assembly.GetTypes())
        {
          if (typeof(LevelLogicComponent).IsAssignableFrom(type))
          {
            list.Add(type);
          }
        }
        _types = list.ToArray();
      }

      var last = Array.IndexOf(_types, target.GetType());
      var index = EditorGUILayout.Popup(last, _types.Select(t => t.Name).ToArray());
      if (index != last)
      {
        if (index != -1)
        {
          Target.gameObject.AddComponent(_types[index]);
        }
        DestroyImmediate(Target);
      }
      else
      {
        base.OnInspectorGUI();
      }
    }
  }
}