using UnityEngine;

namespace Game.Components.MenuNavigation
{
  public class MoveToCurrentNavigationComponent : MonoBehaviour
  {
    public MenuNavigationComponent Navigation;
    public Transform Target;
    public float Speed = 1f;

    public bool FreezeX;
    public bool FreezeY;
    public bool FreezeZ;

    private void Update()
    {
      if (Target != null && Navigation != null && Navigation.Current != null)
      {
        if (Target != Navigation.Current)
        {
          var position = Target.position;
          var newPosition = Vector3.MoveTowards(Target.position, Navigation.Current.transform.position, Speed);
          if (FreezeX) newPosition.x = position.x;
          if (FreezeY) newPosition.y = position.y;
          if (FreezeZ) newPosition.z = position.z;
          Target.position = newPosition;
        }
      }
    }
  }
}
