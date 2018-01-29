using UnityEngine;

namespace Game.Components.MainMenu
{
  public class MainMenuPreviewComponent : MonoBehaviour
  {
    public Camera Camera;

    public void Activate()
    {
      Camera.enabled = true;
      gameObject.SetActive(true);
    }

    public void Deactivate()
    {
      Camera.enabled = false;
      gameObject.SetActive(false);
    }
  }
}
