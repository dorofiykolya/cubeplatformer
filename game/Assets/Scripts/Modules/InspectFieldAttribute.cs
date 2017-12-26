using System;
using System.Reflection;

namespace Game
{
  [AttributeUsage(AttributeTargets.Field)]
  public class InspectFieldAttribute : Attribute
  {
  }

  public class InspectAttribute<T>
  {
    private static FieldInfo[] _fields;

    public static FieldInfo[] Fields
    {
      get
      {
        if (_fields == null)
        {
          _fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField);
        }
        return _fields;
      }
    }
  }
}
