using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components
{
  [RequireComponent(typeof(ScrollRect))]
  public class ScrollRectSelectable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IEventSystemHandler
  {
    public enum ScrollDirection
    {
      Vertical,
      Horizontal
    }

    public float Padding;
    public float MoveSpeed = 8f;
    public ScrollDirection Direction;
    private ScrollRect _scrollRect;
    private bool _pressed;
    private GameObject _lastSelectable;

    public ScrollRect ScrollRect
    {
      get { return _scrollRect ?? (_scrollRect = GetComponent<ScrollRect>()); }
    }

    public RectTransform Viewport
    {
      get { return ScrollRect.viewport; }
    }

    public RectTransform Content
    {
      get { return ScrollRect.content; }
    }


    public virtual void OnPointerDown(PointerEventData eventData)
    {
      _pressed = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
      _pressed = false;
    }

    private void Update()
    {
      var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
      if (currentSelectedGameObject && currentSelectedGameObject.GetComponent<Selectable>())
      {
        Transform selected = null;
        var current = currentSelectedGameObject.transform;
        while (current != null)
        {
          if (current.GetComponent<ScrollRectSelectable>() == this)
          {
            selected = currentSelectedGameObject.transform;
            break;
          }

          current = current.transform.parent;
        }

        if (selected && _lastSelectable != selected)
        {
          var viewportRect = Viewport.rect;
          var viewportMatrix = Viewport.localToWorldMatrix;
          var viewportLeftBottom = viewportMatrix.MultiplyPoint(viewportRect.min);
          var viewportRightTop = viewportMatrix.MultiplyPoint(viewportRect.max);

          var selectedRect = selected.GetComponent<RectTransform>().rect;
          var selectedMatrix = selected.localToWorldMatrix;
          var selectedLeftBottom = selectedMatrix.MultiplyPoint(selectedRect.min);
          var selectedRightTop = selectedMatrix.MultiplyPoint(selectedRect.max);

          if (selectedRightTop.y > viewportRightTop.y)
          {
            var diff = selectedRightTop.y - viewportRightTop.y;
            Content.localPosition -= new Vector3(0, diff / GetScale(Content) * MoveSpeed * Time.deltaTime, 0);
          }
          else if (selectedLeftBottom.y < viewportLeftBottom.y)
          {
            var diff = viewportLeftBottom.y - selectedLeftBottom.y;
            Content.localPosition += new Vector3(0, diff / GetScale(Content) * MoveSpeed * Time.deltaTime, 0);
          }
        }
      }

      if (currentSelectedGameObject == null)
      {
        StopMove();
      }
    }

    public float GetScale(RectTransform transform)
    {
      return transform.lossyScale.y;
    }

    public void StopMove()
    {

    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
      _pressed = true;
      StopMove();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      _pressed = true;
      StopMove();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      _pressed = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
      _pressed = true;
      StopMove();
    }

    public void OnScroll(PointerEventData eventData)
    {
      StopMove();
    }
  }
}
