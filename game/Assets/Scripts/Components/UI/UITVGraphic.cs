using UnityEngine;
using UnityEngine.UI;

namespace Game.Components
{
  public class UITVGraphic : Graphic, ICanvasRaycastFilter
  {
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
      Vector2 localPoint;
      if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, screenPoint, eventCamera, out localPoint))
        return false;
      return true;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
      vh.Clear();
    }
  }
}
