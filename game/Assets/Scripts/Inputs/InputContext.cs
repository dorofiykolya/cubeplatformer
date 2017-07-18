using System;
using UnityEngine;
using System.Collections;
using Utils;

namespace Game.Inputs
{
  public class InputContext
  {
    private Lifetime.Definition _definition;
    private Signal<InputController> _onControllerAdd;
    private Signal<InputController> _onControllerRemove;
    private Signal<InputEvent> _onInput;

    protected InputContext(GameContext context, Lifetime lifetime) : this(context, lifetime, null)
    {

    }

    protected InputContext(GameContext context, Lifetime lifetime, InputContext parent)
    {
      Parent = parent;
      parent.Nested = this;

      _definition = Lifetime.Define(lifetime);
      _onInput = new Signal<InputEvent>(_definition.Lifetime);
      _onControllerAdd = new Signal<InputController>(_definition.Lifetime);
      _onControllerRemove = new Signal<InputController>(_definition.Lifetime);
    }

    protected InputContext Root { get { return Parent != null ? Parent.Root : this; } }
    protected InputContext Parent { get; private set; }
    protected InputContext Nested { get; private set; }
    protected InputContext Last { get { return Nested != null ? Nested.Nested : this; } }

    public virtual InputController[] Controllers { get { return Root == this ? new InputController[0] : Root.Controllers; } }

    public virtual void Subscribe(Lifetime lifetime, GameInput input, Action<InputEvent> listener)
    {
      _onInput.Subscribe(lifetime, (evt) =>
      {
        if (evt.Input == input)
        {
          listener(evt);
        }
      });
    }

    public virtual void Subscribe(Lifetime lifetime, GameInput input, InputPhase phase, Action<InputEvent> listener)
    {
      _onInput.Subscribe(lifetime, (evt) =>
      {
        if (evt.Input == input && evt.Phase == phase)
        {
          listener(evt);
        }
      });
    }

    public virtual void Subscribe(Lifetime lifetime, Action<InputEvent> listener)
    {
      _onInput.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeOnAddController(Lifetime lifetime, Action<InputController> listener)
    {
      Root._onControllerAdd.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeOnRemoveController(Lifetime lifetime, Action<InputController> listener)
    {
      Root._onControllerRemove.Subscribe(lifetime, listener);
    }

    protected virtual void Fire(InputEvent evt)
    {
        _onInput.Fire(evt);
    }

    protected virtual void FireAddController(InputController controller)
    {
        Root._onControllerAdd.Fire(controller);
    }

    protected virtual void FireRemoveController(InputController controller)
    {
        Root._onControllerRemove.Fire(controller);
    }
  }
}