using UnityEngine;

namespace Game.Components.MainMenu
{
  public class MainMenuTVComponent : MonoBehaviour
  {
    public MainMenuTVSceneComponent PreviewScene;
    public MainMenuPreviewComponent Preview;

    public bool Activated { get; private set; }

    public void Activate()
    {
      if (!Activated)
      {
        Activated = true;
        PreviewScene.Activate(Preview);
      }
    }

    public void Deactivate()
    {
      Activated = false;
      PreviewScene.Deactivate(Preview);
    }
  }
}
