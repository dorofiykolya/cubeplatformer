using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.HUDs
{
  public class UIHUDClassicPlayModeComponent : UIHUDComponent
  {
    private Lifetime.Definition _definition;
    private Signal _onClick;

    [SerializeField]
    private Button _button;

    private void Awake()
    {
      _definition = Lifetime.Define(Lifetime.Eternal);
      _onClick = new Signal(_definition.Lifetime);
      _button.onClick.AddListener(_onClick.Fire);
    }

    private void OnDestroy()
    {
      _definition.Terminate();
    }

    public void SubscribeOnClick(Lifetime lifetime, Action listener)
    {
      _onClick.Subscribe(lifetime, listener);
    }
  }
}