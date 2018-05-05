using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Components
{
  public class UITVButton : Selectable, IPointerClickHandler, ISubmitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
  {
    [SerializeField]
    private bool _selectOnEnable;
    [SerializeField]
    private float _threshold;

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();

    private Vector2 _startDrag;
    private bool _isDrag;
    private UISelectionGroupComponent _selectionGroup;

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
      if (eventData.button != PointerEventData.InputButton.Left || _isDrag)
        return;
      this.Press();
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

    public void OnDrag(PointerEventData eventData)
    {
      if (!_isDrag && (eventData.position - _startDrag).magnitude > Threshold)
      {
        _isDrag = true;
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      _startDrag = eventData.position;
      _isDrag = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      _isDrag = false;
    }

    protected override void OnEnable()
    {
      _isDrag = false;
      base.OnEnable();
      if (_selectOnEnable)
      {
        Select();
      }
    }

    protected float Threshold
    {
      get { return _threshold; }
    }

    protected override void Awake()
    {
      base.Awake();
      _selectionGroup = GetComponentInParent<UISelectionGroupComponent>();
    }

    public override void OnSelect(BaseEventData eventData)
    {
      base.OnSelect(eventData);
      if (_selectionGroup != null)
      {
        _selectionGroup.Default = this;
      }
    }
  }
}
