using UnityEngine;

namespace Game.Components.MainMenu
{
  public class MainMenuTVSceneComponent : MonoBehaviour
  {
    public MainMenuPreviewComponent Current;

    public void Activate(MainMenuPreviewComponent preview)
    {
      if (preview != Current)
      {
        if (Current != null)
        {
          Current.Deactivate();
        }
        Current = preview;
        Current.Activate();
      }
    }

    public void Deactivate()
    {
      if (Current != null)
      {
        Current.Deactivate();
        Current = null;
      }
    }

    public void Deactivate(MainMenuPreviewComponent preview)
    {
      if (Current == preview)
      {
        Deactivate();
      }
    }
  }
}
