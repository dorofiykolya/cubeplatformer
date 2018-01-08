using System;
using Game.UI;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Game.Components.MenuNavigation
{
  public class NavigationComponent : MonoBehaviour
  {
    [Header("Target")]
    public Transform Target;
    public MainMenuId Id;
    public Transform SelectedTransform;
    public Transform CameraPivotTransform;

    [Header("Events")]
    public UnityEvent OnSelected;
    public UnityEvent OnUnselected;

    [Header("Navigation")]
    public NavigationComponent Left;
    public NavigationComponent Right;
    public NavigationComponent Top;
    public NavigationComponent Bottom;

    private Signal _onSelected;
    private Signal _onUnselected;
    private Lifetime.Definition _onSelectedDefinition;
    private Lifetime.Definition _onUnselectedDefinition;

    public void SubscribeOnSelected(Lifetime lifetime, Action listener)
    {
      if (_onSelected == null)
      {
        _onSelectedDefinition = Lifetime.Define(Lifetime.Eternal);
        _onSelected = new Signal(_onSelectedDefinition.Lifetime);
      }
      _onSelected.Subscribe(lifetime, listener);
    }

    public void SubscribeOnUnselected(Lifetime lifetime, Action listener)
    {
      if (_onUnselected == null)
      {
        _onUnselectedDefinition = Lifetime.Define(Lifetime.Eternal);
        _onUnselected = new Signal(_onUnselectedDefinition.Lifetime);
      }
      _onUnselected.Subscribe(lifetime, listener);
    }

    private void OnDestroy()
    {
      if (_onSelectedDefinition != null)
      {
        _onSelectedDefinition.Terminate();
        _onSelectedDefinition = null;
      }
      if (_onUnselectedDefinition != null)
      {
        _onUnselectedDefinition.Terminate();
        _onUnselectedDefinition = null;
      }
    }
  }
}
