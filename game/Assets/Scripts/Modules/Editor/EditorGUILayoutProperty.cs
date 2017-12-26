using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
  public class EditorGUILayoutProperty
  {
    private static readonly Dictionary<Key, DrawerEditor> _map = new Dictionary<Key, DrawerEditor>
    {
      { GetKey(typeof(int)), new DrawerEditor((value)=> EditorGUILayout.IntField(value.Label, (int)value.Value)) },
      { GetKey(typeof(int), OptionType.Flag), new DrawerEditor((value)=> EditorGUILayout.EnumFlagsField(value.Label, (Enum)value.Value)) },
      { GetKey(typeof(int), OptionType.Popup), new DrawerEditor((value)=> EditorGUILayout.Popup(value.Label, (int)value.Value, value.Options.Popup)) },
      { GetKey(typeof(int), OptionType.IntPopup), new DrawerEditor((value)=> EditorGUILayout.IntPopup(value.Label, (int)value.Value, value.Options.Popup, value.Options.IntPopup)) },
      { GetKey(typeof(int), OptionType.Range), new DrawerEditor((value)=> EditorGUILayout.IntSlider(value.Label, (int)value.Value, (int)value.Options.MinValue, (int)value.Options.MaxValue)) },
      { GetKey(typeof(int), OptionType.Layer), new DrawerEditor((value)=> EditorGUILayout.LayerField(value.Label, (int)value.Value)) },
      { GetKey(typeof(float)), new DrawerEditor((value)=> EditorGUILayout.FloatField(value.Label, (float)value.Value)) },
      { GetKey(typeof(float), OptionType.Range), new DrawerEditor((value)=> EditorGUILayout.Slider(value.Label, (float)value.Value, (float) value.Options.MinValue, (float) value.Options.MaxValue)) },
      { GetKey(typeof(Vector2)), new DrawerEditor((value)=> EditorGUILayout.Vector2Field(value.Label, (Vector2)value.Value)) },
      { GetKey(typeof(Vector3)), new DrawerEditor((value)=> EditorGUILayout.Vector3Field(value.Label, (Vector3)value.Value)) },
      { GetKey(typeof(Vector4)), new DrawerEditor((value)=> EditorGUILayout.Vector4Field(value.Label, (Vector4)value.Value)) },
      { GetKey(typeof(string)), new DrawerEditor((value)=> EditorGUILayout.TextField(value.Label, (string)value.Value)) },
      { GetKey(typeof(string), OptionType.Password), new DrawerEditor((value)=> EditorGUILayout.PasswordField(value.Label, (string)value.Value)) },
      { GetKey(typeof(Bounds)), new DrawerEditor((value)=> EditorGUILayout.BoundsField(value.Label, (Bounds)value.Value)) },
      { GetKey(typeof(Color)), new DrawerEditor((value)=> EditorGUILayout.ColorField(value.Label, (Color)value.Value)) },
      { GetKey(typeof(AnimationCurve)), new DrawerEditor((value)=> EditorGUILayout.CurveField(value.Label, (AnimationCurve)value.Value)) },
      { GetKey(typeof(Enum)), new DrawerEditor((value)=> EditorGUILayout.EnumPopup(value.Label, (Enum)value.Value)) },
      { GetKey(typeof(Enum), OptionType.Flag), new DrawerEditor((value)=> EditorGUILayout.EnumFlagsField(value.Label, (Enum)value.Value)) },
      { GetKey(typeof(Rect)), new DrawerEditor((value)=> EditorGUILayout.RectField(value.Label, (Rect)value.Value)) },
      { GetKey(typeof(RectInt)), new DrawerEditor((value)=> EditorGUILayout.RectIntField(value.Label, (RectInt)value.Value)) },
      { GetKey(typeof(bool)), new DrawerEditor((value)=> EditorGUILayout.Toggle(value.Label, (bool)value.Value)) },
      { GetKey(typeof(double)), new DrawerEditor((value)=> EditorGUILayout.DoubleField(value.Label, (double)value.Value)) },
      { GetKey(typeof(long)), new DrawerEditor((value)=> EditorGUILayout.LongField(value.Label, (long)value.Value)) },
      { GetKey(typeof(UnityEngine.Object)), new DrawerEditor((value)=> EditorGUILayout.ObjectField(value.Label, (UnityEngine.Object)value.Value, typeof(UnityEngine.Object), value.AllowSceneObject)) },
    };

    private static Key GetKey(Type type, OptionType optionType = OptionType.None)
    {
      return new Key
      {
        Type = type,
        OptionType = optionType
      };
    }

    public static object DrawProperty(string label, object value, Type type, bool allowSceneObject = false, Options options = new Options())
    {
      DrawerEditor data;
      if (_map.TryGetValue(GetKey(type, options.Type), out data))
      {
        return data.Draw(label, value, type, allowSceneObject, options);
      }

      if (typeof(UnityEngine.Object).IsAssignableFrom(type))
      {
        return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, allowSceneObject);
      }

      return value;
    }

    [Flags]
    public enum OptionType
    {
      None,
      Password,
      Range,
      Flag,
      Layer,
      Popup,
      IntPopup
    }

    public struct Options
    {
      public OptionType Type;
      public float MinValue;
      public float MaxValue;
      public string[] Popup;
      public int[] IntPopup;
    }

    private struct Key : IEquatable<Key>
    {
      public Type Type;
      public OptionType OptionType;

      public bool Equals(Key other)
      {
        return Equals(Type, other.Type) && OptionType == other.OptionType;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        return obj is Key && Equals((Key)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (int)OptionType;
        }
      }
    }

    private struct DrawerData
    {
      public string Label;
      public object Value;
      public Type Type;
      public bool AllowSceneObject;
      public Options Options;
    }

    private class DrawerEditor
    {
      private readonly Func<DrawerData, object> _function;

      public DrawerEditor(Func<DrawerData, object> function)
      {
        _function = function;
      }

      public object Draw(string label, object value, Type type, bool allowSceneObject, Options options)
      {
        return _function(new DrawerData
        {
          Label = label,
          Type = type,
          Value = value,
          AllowSceneObject = allowSceneObject,
          Options = options
        });
      }
    }
  }
}
