using Game.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
  public class GameStarterScene : MonoBehaviour
  {
    private void Awake()
    {
      if (GameContext.Context == null)
      {

        SceneManager.LoadSceneAsync(GameScenes.Main);
      }
    }
  }
}
