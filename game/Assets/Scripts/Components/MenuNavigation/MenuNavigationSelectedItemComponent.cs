using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Components.MenuNavigation
{
  public class MenuNavigationSelectedItemComponent : MonoBehaviour
  {
    public Transform Target;
    public MenuNavigationComponent Navigation;
    public float MoveSpeed = 50f;
    public float ScaleSpeed = 50f;
    public float RotationSpeed = 360f;

    private void Awake()
    {
      if (Navigation != null && Navigation.Current != null)
      {
        var current = Navigation.Current.SelectedTransform;
        if (current != null)
        {
          Target.position = current.position;
          Target.localScale = current.localScale;
          Target.rotation = current.rotation;
        }
      }
    }

    private void Update()
    {
      if (Target != null && Navigation != null && Navigation.Current != null)
      {
        var current = Navigation.Current.SelectedTransform;
        if (current != null)
        {
          Target.position = Vector3.MoveTowards(Target.position, current.position, MoveSpeed * Time.deltaTime);
          Target.localScale = Vector3.MoveTowards(Target.localScale, current.localScale, ScaleSpeed * Time.deltaTime);
          Target.rotation = Quaternion.RotateTowards(Target.rotation, current.rotation, RotationSpeed * Time.deltaTime);
        }
      }
    }
  }
}
