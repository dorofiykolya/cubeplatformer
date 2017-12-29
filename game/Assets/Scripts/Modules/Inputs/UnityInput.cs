using System.Collections.Generic;
using UnityEngine;

namespace Game
{
  public class UnityInput
  {
    private static readonly Dictionary<UnityInputAxis, string> _cache = new Dictionary<UnityInputAxis, string>();

    public static float GetAxis(UnityInputAxis axis)
    {
      string result;
      if (!_cache.TryGetValue(axis, out result))
      {
        _cache[axis] = result = axis.ToString();
      }
      return Input.GetAxis(result);
    }
  }
}
