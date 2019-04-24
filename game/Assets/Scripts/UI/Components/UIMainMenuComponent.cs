using Game.Components.MenuNavigation;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.UI.Components
{
  public class UIMainMenuComponent : MonoBehaviour
  {
    private static bool _firstStart = true;

    public Camera MainCamera;
    public Camera[] Cameras;
    public MenuNavigationComponent Navigation;
    public PlayableDirector PlayableDirector;

    private void Awake()
    {
      PlayableDirector.playOnAwake = _firstStart;
      if (!_firstStart)
      {
        PlayableDirector.Stop();
      }
      _firstStart = false;
    }

    public void ActiveCameras(bool value)
    {
      foreach (var cam in Cameras)
      {
        cam.enabled = value;
      }
    }
  }
}
