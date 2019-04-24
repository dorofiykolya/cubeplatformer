using Game.UI.Windows;
using System;
using System.Collections.Generic;

namespace Game.UI.Providers
{
  public class UIWindowMap
  {
    public Type Type;
    public string Path;
    public bool IsFullscreen;

    public UIWindowMap(Type type, string path, bool isFullscreen)
    {
      Type = type;
      Path = path;
      IsFullscreen = isFullscreen;
    }
  }

  public class UIWindowsProvider
  {
    public IEnumerable<UIWindowMap> GetWindows()
    {
      yield return new UIWindowMap(typeof(UISettingsWindow), "Windows/Settings", false);
      yield return new UIWindowMap(typeof(UILevelSelectWindow), "Windows/LevelSelect", true);
      yield return new UIWindowMap(typeof(UIErrorWindow), "Windows/Error", false);
      yield return new UIWindowMap(typeof(UITutorialWindow), "Windows/Tutorial", true);
      yield break;
    }
  }
}