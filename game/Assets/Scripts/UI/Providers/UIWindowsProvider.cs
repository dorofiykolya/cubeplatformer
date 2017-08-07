using System;
using System.Collections.Generic;
using Game.UI.Windows;

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