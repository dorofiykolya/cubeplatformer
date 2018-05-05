using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Components.UI
{
  public class UIClickComponent : Selectable, IPointerClickHandler, ISelectHandler, IUpdateSelectedHandler
  {
    public void OnPointerClick(PointerEventData eventData)
    {
      Debug.Log("CLICK!!!");//GraphicRaycaster
    }

    public void OnSelect(BaseEventData eventData)
    {
      Debug.Log("SELECT: " + (eventData.selectedObject == gameObject));//Image
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
      Debug.Log("UPDATE SELECT: " + (eventData.selectedObject == gameObject));
    }
  }
}
