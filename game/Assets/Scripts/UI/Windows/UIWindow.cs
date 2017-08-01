using System;
using Utils;

namespace Game.UI.Components
{
  public abstract class UIWindow<T> : UIWindow where T : UIWindowComponent
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

  public abstract class UIWindow
  {
    private UIWindowComponent _component;
    private Lifetime _lifetime;

    public abstract Type ComponentType { get; }

    public UIWindowComponent Component
    {
      get { return _component; }
    }

    protected Lifetime Lifetime { get { return _lifetime; } }
    protected abstract void Initialize();
    protected abstract void OnOpen();
    protected abstract void OnClose();

    [Initialize]
    private void InitializeWindowInternal(Lifetime lifetime, UIWindowComponent component)
    {
      _lifetime = lifetime;
      _component = component;
      Initialize();
    }

    [WindowOpen]
    private void WindowOpenInternal()
    {
      OnOpen();
    }

    [WindowClose]
    private void WindowCloseInternal()
    {
      OnClose();
    }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class WindowOpenAttribute : Attribute { }
  [AttributeUsage(AttributeTargets.Method)]
  public class WindowCloseAttribute : Attribute { }
}