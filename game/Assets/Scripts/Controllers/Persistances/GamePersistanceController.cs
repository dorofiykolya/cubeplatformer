using UnityEngine;

namespace Game.Controllers
{
  public class GamePersistanceController : GameController
  {
    public int LastClassicLevel
    {
      get { return PlayerPrefs.GetInt(GetType().Name + ":" + "LastClassicLevel", 0); }
      set { PlayerPrefs.SetInt(GetType().Name + ":" + "LastClassicLevel", value); }
    }

    public void SetInt(string key, int value)
    {
      PlayerPrefs.SetInt(key, value);
    }

    public int GetInt(string key, int defaultValue)
    {
      return PlayerPrefs.GetInt(key, defaultValue);
    }

    public int GetInt(string key)
    {
      int defaultValue = 0;
      return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetFloat(string key, float value)
    {
      PlayerPrefs.SetFloat(key, value);
    }

    public float GetFloat(string key, float defaultValue)
    {
      return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public float GetFloat(string key)
    {
      float defaultValue = 0.0f;
      return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
      PlayerPrefs.SetString(key, value);
    }

    public string GetString(string key, string defaultValue)
    {
      return PlayerPrefs.GetString(key, defaultValue);
    }

    public string GetString(string key)
    {
      string defaultValue = string.Empty;
      return PlayerPrefs.GetString(key, defaultValue);
    }

    public bool HasKey(string key)
    {
      return PlayerPrefs.HasKey(key);
    }

    public void DeleteKey(string key)
    {
      PlayerPrefs.DeleteKey(key);
    }

    public void DeleteAll()
    {
      PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
      PlayerPrefs.Save();
    }

  }
}