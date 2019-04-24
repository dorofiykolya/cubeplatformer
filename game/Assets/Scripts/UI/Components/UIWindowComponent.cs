using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Windows
{
  public class UIWindowComponent : MonoBehaviour
  {
    private Lifetime.Definition _definition;
    private Signal _onClose;

    public UIWindowComponent()
    {
      InitializeComponent();
    }

    public Lifetime Lifetime
    {
      get { return _definition.Lifetime; }
    }

    private void InitializeComponent()
    {
      _definition = _definition ?? Lifetime.Define(Lifetime.Eternal);
      _onClose = _onClose ?? new Signal(Lifetime.Eternal);
    }

    private void OnEnable()
    {
      InitializeComponent();
    }

    private void OnDisable()
    {
      _definition.Terminate();
      _definition = null;
      _onClose = null;
    }

    public void SubscribeOnClose(Lifetime lifetime, Action listener)
    {
      InitializeComponent();
      _onClose.Subscribe(lifetime, listener);
    }

    [UILink]
    public void FireClickClose()
    {
      _onClose.Fire();
    }
  }
}