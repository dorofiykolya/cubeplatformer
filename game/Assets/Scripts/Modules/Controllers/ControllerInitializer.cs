using UnityEngine;
using System.Collections;
using System.Reflection;
using Utils;

namespace Game
{
  public static class ControllerInitializer
  {
    private static MethodInfo[] _initializeMethods;
    private static MethodInfo[] _preinitializeMethods;

    public static void Initialize(Controller controller)
    {
      if (_initializeMethods == null)
      {
        _initializeMethods = MethodAttributeUtil.GetMethods(typeof(Controller), typeof(InitializeAttribute));
        Assert2.IsTrue(_initializeMethods.Length > 0);
      }
      foreach (var method in _initializeMethods)
      {
        method.Invoke(controller, null);
      }
    }

    public static void Preinitialize(Controller controller)
    {
      if (_preinitializeMethods == null)
      {
        _preinitializeMethods = MethodAttributeUtil.GetMethods(typeof(Controller), typeof(PreinitializeAttribute));
        Assert2.IsTrue(_preinitializeMethods.Length > 0);
      }
      foreach (var method in _preinitializeMethods)
      {
        method.Invoke(controller, null);
      }
    }
  }
}