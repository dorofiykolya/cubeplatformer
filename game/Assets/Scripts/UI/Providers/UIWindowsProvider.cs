using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.UI.Controllers;

namespace Game.UI.Providers
{
  public struct UIWindowMap
  {
    public Type Type;
    public string Path;

    public UIWindowMap(Type type, string path)
    {
      Type = type;
      Path = path;
    }
  }

  public class UIWindowsProvider
  {
    public IEnumerable<UIWindowMap> GetWindows()
    {
      yield return new UIWindowMap(typeof(UIMainMenuWindow), "Windows/MainMenu");
    }
  }
}