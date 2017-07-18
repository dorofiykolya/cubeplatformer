using System;
using UnityEngine;
using System.Collections;
using Utils;

namespace Game.Inputs
{
  public class InputContext
  {
    private Lifetime.Definition _definition;
    private Signal _onControllersChanged;
    private Signal<InputEvent> _onInput;

    protected InputContext(GameContext context, Lifetime lifetime) : this(context, lifetime, null)
    {

    }

    protected InputContext(GameContext context, Lifetime lifetime, InputContext parent)
    {
      Parent = parent;
      parent.Nested = this;

      _definition = Lifetime.Define(lifetime);
      _onControllersChanged = new Signal(_definition.Lifetime);
      _onInput = new Signal<InputEvent>(_definition.Lifetime);
    }

    protected InputContext Root { get { return Parent != null ? Parent.Root : this; } }
    protected InputContext Parent { get; private set; }
    protected InputContext Nested { get; private set; }
    protected InputContext Last { get { return Nested != null ? Nested.Nested : this; } }

    public virtual int Controllers { get { return Root == this ? 0 : Root.Controllers; } }

    public virtual void SubscribeOnControllersChanged(Lifetime lifetime, Action listener)
    {

    }

    public virtual void Subscribe(Lifetime lifetime, GameInput input, Action<InputEvent> listener)
    {

    }

    protected virtual void FireControllersChanged()
    {

    }

    protected virtual void Fire(InputEvent evt)
    {

    }
  }
}