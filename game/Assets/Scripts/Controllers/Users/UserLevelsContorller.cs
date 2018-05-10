using System.Reflection;
using Injection;
using UnityEngine;

namespace Game.Controllers
{
  public class UserLevelsContorller : GameController
  {
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private GamePersistanceController _persistanceController;

    public UserLevelData GetData(int level, int sublevel)
    {
      var data = _persistanceController.GetString(GetPersistanceKey(level, sublevel));
      if (data != null)
      {
        return JsonUtility.FromJson<UserLevelData>(data);
      }

      return null;
    }

    public void SaveLevel(UserLevelData data)
    {
      var json = JsonUtility.ToJson(data);
      _persistanceController.SetString(GetPersistanceKey(data.Level, data.SubLevel), json);
      _persistanceController.Save();
    }

    private string GetPersistanceKey(int level, int sublevel)
    {
      return GetType().Name + ':' + MethodBase.GetCurrentMethod().Name + ':' + level + ':' + sublevel;
    }
  }
}
