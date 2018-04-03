using Game.Components.MenuNavigation;
using UnityEngine;

namespace Game.UI.Components
{
  public class UIMainMenuComponent : MonoBehaviour
  {
    public Camera MainCamera;
    public Camera[] Cameras;
    public MenuNavigationComponent Navigation;

    public void ActiveCameras(bool value)
    {
      foreach (var cam in Cameras)
      {
        cam.enabled = value;
      }
    }
  }
}
