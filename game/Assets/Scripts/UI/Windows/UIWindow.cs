using System;
using Utils;

namespace Game.UI.Windows
{
  public abstract class UIWindow<TComponent, TData> : UIWindow where TComponent : UIWindowComponent where TData : UIWindowData
  {
    public override Type ComponentType
    {
      get { return typeof(TComponent); }
    }

    public new TComponent Component
    {
      get { return base.Component as TComponent; }
    }

    public new TData Data
    {
      get { return base.Data as TData; }
    }
  }

  public abstract class UIWindow
  {
    private UIWindowComponent _component;
    private Lifetime.Definition _lifetimeDefinition;
    private UIWindowData _data;

    public abstract Type ComponentType { get; }

    public UIWindowData Data
    {
      get { return _data; }
    }

    public UIWindowComponent Component
    {
      get { return _component; }
    }

    protected Lifetime Lifetime { get { return _lifetimeDefinition.Lifetime; } }
    protected abstract void OnInitialize();
    protected abstract void OnOpen();
    protected abstract void OnClose();

    protected void Close()
    {
      _lifetimeDefinition.Terminate();
    }

    [Initialize]
    private void InitializeWindowInternal(Lifetime.Definition lifetimeDefinition, UIWindowComponent component, UIWindowData data)
    {
      _lifetimeDefinition = lifetimeDefinition;
      _component = component;
      _data = data;
      OnInitialize();
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