using UnityEngine;

namespace Game.Managers
{
  public class GamePersistanceManager : GameManager
  {
    public int LastClassicLevel
    {
      get { return PlayerPrefs.GetInt(GetType().Name + ":" + "LastClassicLevel", 0); }
      set { PlayerPrefs.SetInt(GetType().Name + ":" + "LastClassicLevel", value); }
    }

  }
}