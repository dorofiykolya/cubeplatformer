using System;
using System.Collections.Generic;
using Game.UI.HUDs;

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
    }
  }
}
