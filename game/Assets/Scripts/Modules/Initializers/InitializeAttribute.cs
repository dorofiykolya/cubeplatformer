using System;
using System.Reflection;
using Utils;

namespace Game
{
  [AttributeUsage(AttributeTargets.Method)]
  public class InitializeAttribute : Attribute
  {
    public static MethodInfo[] GetMethods(Type type)
    {
      return MethodAttributeUtil.GetMethods(type, typeof(InitializeAttribute));
    }
  }
}