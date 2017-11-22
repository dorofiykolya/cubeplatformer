using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Inputs
{
  public interface IInputName
  {
    string Name { get; }
  }

  public class InputNameMapper<T> where T : IInputName
  {
    private static Dictionary<string, T> _result;
    private static T[] _collection;

    public static T Get(string name)
    {
      Initialize();
      T result;
      _result.TryGetValue(name, out result);
      return result;
    }

    public static T[] Collection
    {
      get
      {
        Initialize();
        return _collection;
      }
    }

    private static void Initialize()
    {
      if (_result == null)
      {
        _result = new Dictionary<string, T>();
        var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
        foreach (var fieldInfo in fields)
        {
          var value = (T)fieldInfo.GetValue(typeof(T));
          if (value != null)
          {
            var input = ((T)fieldInfo.GetValue(typeof(T)));
            _result[value.Name] = input;
          }
        }
        _collection = _result.Values.ToArray();
      }
    }
  }

}
