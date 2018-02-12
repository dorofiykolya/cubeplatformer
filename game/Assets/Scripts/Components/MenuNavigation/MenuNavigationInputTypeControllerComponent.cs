using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.MenuNavigation
{
  public class MenuNavigationInputTypeControllerComponent : MonoBehaviour
  {
    public UnityEvent OnSetTouch;
    public UnityEvent OnSetController;

    public void SetTouchInput()
    {
      OnSetTouch.Invoke();
    }

    public void SetControllerInput()
    {
      OnSetController.Invoke();
    }
  }
}
