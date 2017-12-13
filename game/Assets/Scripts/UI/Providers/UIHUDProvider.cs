using System;
using System.Collections.Generic;
using Game.UI.HUDs;
using Game.UI.Windows;

namespace Game.UI.Providers
{
  public struct UIHUDMap
  {
    public Type Type;
    public string Path;

    public UIHUDMap(Type type, string path)
    {
      Type = type;
      Path = path;
    }
  }

  public class UIHUDProvider
  {
    public IEnumerable<UIHUDMap> GetMap()
    {
      yield return new UIHUDMap(typeof(UIHUDClassicPlayMode), "HUDs/ClassicPlayMode");
      yield return new UIHUDMap(typeof(UIHUDMainMenu), "Windows/MainMenu");
      yield return new UIHUDMap(typeof(UIHUDSceneMainManu), null);
    }
  }
}
