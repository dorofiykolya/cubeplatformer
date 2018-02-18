using System;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.Inputs
{
  public class InputContext
  {
    private Lifetime.Definition _definition;
    private readonly Signal<InputController> _onControllerAdd;
    private readonly Signal<InputController> _onControllerActivated;
    private readonly Signal<InputController> _onControllerDeactivated;
    private readonly Signal<InputController> _onControllerRemove;
    private readonly Signal<InputEvent> _onInput;
    private readonly Signal<TouchInputEvent> _onTouchInput;

    protected InputContext(GameContext context, Lifetime lifetime) : this(context, lifetime, null)
    {

    }

    protected InputContext(GameContext context, Lifetime lifetime, InputContext parent)
    {
      Parent = parent;
      if (parent != null) parent.Nested = this;

      _definition = Lifetime.Define(lifetime);
      _onInput = new Signal<InputEvent>(_definition.Lifetime);
      _onControllerAdd = new Signal<InputController>(_definition.Lifetime);
      _onControllerRemove = new Signal<InputController>(_definition.Lifetime);
      _onControllerActivated = new Signal<InputController>(_definition.Lifetime);
      _onControllerDeactivated = new Signal<InputController>(_definition.Lifetime);
      _onTouchInput = new Signal<TouchInputEvent>(_definition.Lifetime);

      _definition.Lifetime.AddAction(() =>
      {
        if (Parent != null)
        {
          Parent.Nested = Nested;
          Parent = null;
        }
      });
    }

    protected InputContext Root { get { return Parent != null ? Parent.Root : this; } }
    protected InputContext Parent { get; private set; }
    protected InputContext Nested { get; private set; }
    protected InputContext Last { get { return Nested != null ? Nested.Last : this; } }

    public Lifetime Lifetime { get { return _definition.Lifetime; } }
    public InputContext Current { get { return Last; } }

    public virtual InputController[] Controllers { get { return Root == this ? new InputController[0] : Root.Controllers; } }

    public virtual InputController GetController(int id)
    {
      return Controllers.FirstOrDefault(c => c.Id == id);
    }

    public virtual InputController GetController(string name)
    {
      return Controllers.FirstOrDefault(c => c.Name == name);
    }

    public virtual void SubscribeTouch(Lifetime lifetime, Action<TouchInputEvent> listener)
    {
      _onTouchInput.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeTouch(Lifetime lifetime, TouchPhase phase, Action<TouchInputEvent> listener)
    {
      _onTouchInput.Subscribe(lifetime, evt =>
      {
        if (evt.Phase == phase)
        {
          listener(evt);
        }
      });
    }

    public virtual void SubscribeTouch(Lifetime lifetime, int id, TouchPhase phase, Action<TouchInputEvent> listener)
    {
      _onTouchInput.Subscribe(lifetime, evt =>
      {
        if (evt.Id == id && evt.Phase == phase)
        {
          listener(evt);
        }
      });
    }

    public virtual void Subscribe(Lifetime lifetime, GameInput input, InputUpdate update, Action<InputEvent> listener)
    {
      _onInput.Subscribe(lifetime, evt =>
      {
        if (evt.Input == input && evt.Update == update)
        {
          listener(evt);
        }
      });
    }

    public virtual void Subscribe(Lifetime lifetime, GameInput input, InputPhase phase, InputUpdate update, Action<InputEvent> listener)
    {
      _onInput.Subscribe(lifetime, evt =>
      {
        if (evt.Input == input && evt.Phase == phase && evt.Update == update)
        {
          listener(evt);
        }
      });
    }

    public virtual void Subscribe(Lifetime lifetime, InputUpdate update, Action<InputEvent> listener)
    {
      _onInput.Subscribe(lifetime, evt =>
      {
        if (evt.Update == update)
        {
          listener(evt);
        }
      });
    }

    public virtual void SubscribeOnAddController(Lifetime lifetime, Action<InputController> listener)
    {
      _onControllerAdd.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeOnRemoveController(Lifetime lifetime, Action<InputController> listener)
    {
      _onControllerRemove.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeOnActivatedController(Lifetime lifetime, Action<InputController> listener)
    {
      _onControllerActivated.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeOnDeactivatedController(Lifetime lifetime, Action<InputController> listener)
    {
      _onControllerDeactivated.Subscribe(lifetime, listener);
    }

    protected virtual void FireEvent(TouchInputEvent evt)
    {
      Last._onTouchInput.Fire(evt);
    }

    protected virtual void FireEvent(InputEvent evt)
    {
      Last._onInput.Fire(evt);
    }

    protected virtual void FireAddController(InputController controller)
    {
      var current = Root;
      while (current != null)
      {
        current._onControllerAdd.Fire(controller);
        current = current.Nested;
      }
    }

    protected virtual void FireRemoveController(InputController controller)
    {
      var current = Root;
      while (current != null)
      {
        current._onControllerRemove.Fire(controller);
        current = current.Nested;
      }
    }

    protected virtual void FireActivateController(InputController controller)
    {
      var current = Root;
      while (current != null)
      {
        current._onControllerActivated.Fire(controller);
        current = current.Nested;
      }
    }

    protected virtual void FireDeactiveController(InputController controller)
    {
      var current = Root;
      while (current != null)
      {
        current._onControllerDeactivated.Fire(controller);
        current = current.Nested;
      }
    }
  }
}