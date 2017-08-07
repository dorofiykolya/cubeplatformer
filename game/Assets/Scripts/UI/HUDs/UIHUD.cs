using System;
using Utils;

namespace Game.UI.HUDs
{
  public abstract class UIHUD<T> : UIHUD where T : UIHUDComponent
  {
    public override Type ComponentType
    {
      get { return typeof(T); }
    }

    public new T Component
    {
      get { return base.Component as T; }
    }
  }

  public abstract class UIHUD
  {
    private UIHUDComponent _component;
    private Lifetime.Definition _lifetimeDefinition;

    public abstract Type ComponentType { get; }

    public UIHUDComponent Component
    {
      get { return _component; }
    }

    protected Lifetime Lifetime { get { return _lifetimeDefinition.Lifetime; } }
    protected abstract void Initialize();
    protected abstract void OnOpen();
    protected abstract void OnClose();

    protected void Close()
    {
      _lifetimeDefinition.Terminate();
    }

    [Initialize]
    private void InitializeWindowInternal(Lifetime.Definition lifetimeDefinition, UIHUDComponent component)
    {
      _lifetimeDefinition = lifetimeDefinition;
      _component = component;
      Initialize();
    }

    [UIHUDOpen]
    private void WindowOpenInternal()
    {
      OnOpen();
    }

    [UIHUDClose]
    private void WindowCloseInternal()
    {
      OnClose();
    }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class UIHUDOpenAttribute : Attribute { }
  [AttributeUsage(AttributeTargets.Method)]
  public class UIHUDCloseAttribute : Attribute { }
}