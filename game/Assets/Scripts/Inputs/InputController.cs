using System;
using Utils;

namespace Game.Inputs
{
  public class InputController
  {
    private readonly Signal<InputController> _onActivate;
    private readonly Signal<InputController> _onDeactivate;
    private bool _active;

    public InputController(string name, int id, Lifetime lifetime)
    {
      _onActivate = new Signal<InputController>(lifetime);
      _onDeactivate = new Signal<InputController>(lifetime);

      Id = id;
      Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }

    public bool Active
    {
      get { return _active; }
      set
      {
        if (_active != value)
        {
          _active = value;
          if (_active)
          {
            _onActivate.Fire(this);
          }
          else
          {
            _onDeactivate.Fire(this);
          }
        }
      }
    }

    public void SubscribeOnActivate(Lifetime lifetime, Action<InputController> listener)
    {
      _onActivate.Subscribe(lifetime, listener);
    }

    public void SubscribeOnDeactivate(Lifetime lifetime, Action<InputController> listener)
    {
      _onDeactivate.Subscribe(lifetime, listener);
    }
  }
}