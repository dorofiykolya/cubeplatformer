using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitBenderGames;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Components
{
  public class UITVButton : Selectable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISubmitHandler, IEventSystemHandler
  {
    public UnityEvent OnDown;
    public UnityEvent OnUp;
    public float Threshold;
    public MobileTouchCamera TouchCamera;

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();

    private Vector3 _lastPosition;

    /// <summary>
    ///   <para>UnityEvent that is triggered when the button is pressed.</para>
    /// </summary>
    public Button.ButtonClickedEvent onClick
    {
      get { return this.m_OnClick; }
      set { this.m_OnClick = value; }
    }

    private void Press()
    {
      if (!this.IsActive() || !this.IsInteractable())
        return;
      UISystemProfilerApi.AddMarker("Button.onClick", (UnityEngine.Object)this);
      this.m_OnClick.Invoke();
    }

    /// <summary>
    ///   <para>Registered IPointerClickHandler callback.</para>
    /// </summary>
    /// <param name="eventData">Data passed in (Typically by the event system).</param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
      //if (eventData.button != PointerEventData.InputButton.Left)
      //  return;
      //this.Press();
    }

    /// <summary>
    ///   <para>Registered ISubmitHandler callback.</para>
    /// </summary>
    /// <param name="eventData">Data passed in (Typically by the event system).</param>
    public virtual void OnSubmit(BaseEventData eventData)
    {
      this.Press();
      if (!this.IsActive() || !this.IsInteractable())
        return;
      this.DoStateTransition(Selectable.SelectionState.Pressed, false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      if (eventData.selectedObject == gameObject)
      {
        _lastPosition = TouchCamera.Cam.transform.position;
        OnDown.Invoke();
      }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      OnUp.Invoke();
      if (eventData.button == PointerEventData.InputButton.Left)
      {
        var diff = TouchCamera.Cam.transform.position - _lastPosition;
        if (diff.magnitude <= Threshold)
        {
          Press();
        }
      }
    }
  }
}
