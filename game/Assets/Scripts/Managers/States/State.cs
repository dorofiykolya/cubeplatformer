using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game
{
  public class State : IEquatable<State>
  {
    private static readonly Dictionary<Type, Dictionary<int, State>> Collection = new Dictionary<Type, Dictionary<int, State>>();

    private readonly State _parent;
    private readonly string _name;
    private readonly int _value;

    public State(string name, int value, State parent = null)
    {
      _name = name;
      _value = value;
      _parent = parent;
    }

    public static bool operator ==(State s1, State s2)
    {
      if (ReferenceEquals(null, s1))
      {
        return ReferenceEquals(null, s2);
      }
      return s1.Equals(s2);
    }

    public static bool operator !=(State s1, State s2)
    {
      if (ReferenceEquals(null, s1))
      {
        return !ReferenceEquals(null, s2);
      }
      return !s1.Equals(s2);
    }

    public static bool operator ==(State s1, int s2)
    {
      if (ReferenceEquals(null, s1)) return false;
      return s1.Equals(s2);
    }

    public static bool operator !=(State s1, int s2)
    {
      if (ReferenceEquals(null, s1)) return true;
      return !s1.Equals(s2);
    }

    public static bool operator ==(int s1, State s2)
    {
      if (ReferenceEquals(null, s2)) return false;
      return s2.Equals(s1);
    }

    public static bool operator !=(int s1, State s2)
    {
      if (ReferenceEquals(null, s2)) return true;
      return !s2.Equals(s1);
    }

    public static T GetState<T>(int value) where T : State
    {
      return GetState(typeof(T), value) as T;
    }

    public static State GetState(Type type, int value)
    {
      Dictionary<int, State> dict;
      if (!Collection.TryGetValue(type, out dict))
      {
        dict = new Dictionary<int, State>();

        var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);

        foreach (var fieldInfo in fieldInfos)
        {
          var pt = fieldInfo.GetValue(type) as State;
          if (pt != null)
          {
            dict.Add(pt._value, pt);
          }
        }
        Collection.Add(type, dict);
      }
      State result;
      dict.TryGetValue(value, out result);
      return result;
    }

    public static IEnumerable<T> GetStates<T>() where T : State
    {
      var type = typeof(T);
      Dictionary<int, State> dict;
      if (!Collection.TryGetValue(type, out dict))
      {
        dict = new Dictionary<int, State>();

        var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);

        foreach (var fieldInfo in fieldInfos)
        {
          var pt = fieldInfo.GetValue(type) as State;
          if (pt != null)
          {
            dict.Add(pt._value, pt);
          }
        }
        Collection.Add(type, dict);
      }
      return dict.Values.Cast<T>();
    }

    public bool Is(State type)
    {
      State target = this;
      while (!ReferenceEquals(target, null))
      {
        if (ReferenceEquals(target, type) || (target._value == type._value))
        {
          return true;
        }

        target = target._parent;
      }
      return false;
    }

    public bool Is(params State[] types)
    {
      foreach (var type in types)
      {
        if (Is(type))
        {
          return true;
        }
      }
      return false;
    }

    protected bool Equals(State type)
    {
      if (ReferenceEquals(null, type)) return false;
      if (ReferenceEquals(this, type)) return true;
      return Is(type);
    }

    public bool Equals(int value)
    {
      return Equals(GetState(GetType(), value));
    }

    public State Parent
    {
      get { return _parent; }
    }

    public string Name
    {
      get { return _name; }
    }

    public int Value
    {
      get { return _value; }
    }

    public override string ToString()
    {
      return _name;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((State)obj);
    }

    public bool ValueEquals(State state)
    {
      if (ReferenceEquals(null, state)) return false;
      return Value == state.Value;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (_name != null ? _name.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ _value;
        return hashCode;
      }
    }

    bool IEquatable<State>.Equals(State other)
    {
      return this.Equals(other);
    }
  }
}
