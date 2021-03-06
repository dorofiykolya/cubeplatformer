﻿using System;
using System.Reflection;
using Utils;

namespace Game
{
  [AttributeUsage(AttributeTargets.Method)]
  public class PreinitializeAttribute : Attribute
  {
    public static MethodInfo[] GetMethods(Type type)
    {
      return MethodAttributeUtil.GetMethods(type, typeof(PreinitializeAttribute));
    }
  }
}