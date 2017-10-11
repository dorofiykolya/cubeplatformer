using System;
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

      _definition.Lifetime.AddAction(() =>
      {
        if (Parent != null)
        {
          Parent.Nested = null;
          Parent = null;
        }
      });
    }

    protected InputContext Root { get { return Parent != null ? Parent.Root : this; } }
    protected InputContext Parent { get; private set; }
    protected InputContext Nested { get; private set; }
    protected InputContext Last { get { return Nested != null ? Nested.Last : this; } }

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

    public virtual void SubscribeOnActivatedController(Lifetime lifetime, Action<InputController> listener)
    {
      Root._onControllerActivated.Subscribe(lifetime, listener);
    }

    public virtual void SubscribeOnDeactivatedController(Lifetime lifetime, Action<InputController> listener)
    {
      Root._onControllerDeactivated.Subscribe(lifetime, listener);
    }

    protected virtual void FireEvent(InputEvent evt)
    {
      Last._onInput.Fire(evt);
    }

    protected virtual void FireAddController(InputController controller)
    {
      Root._onControllerAdd.Fire(controller);
    }

    protected virtual void FireRemoveController(InputController controller)
    {
      Root._onControllerRemove.Fire(controller);
    }

    protected virtual void FireActivateController(InputController controller)
    {
      Root._onControllerActivated.Fire(controller);
    }

    protected virtual void FireDeactiveController(InputController controller)
    {
      Root._onControllerDeactivated.Fire(controller);
    }
  }
}