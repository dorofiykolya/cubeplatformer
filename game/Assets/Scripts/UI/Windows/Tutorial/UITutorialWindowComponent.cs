using Utils;

namespace Game.UI.Windows
{
  public class UITutorialWindowComponent : UIWindowComponent
  {
    private Signal _onOk;

    private void Awake()
    {
      _onOk = new Signal(Lifetime);
    }

    public ISignalSubsribe Ok
    {
      get { return _onOk; }
    }

    public void FireOk()
    {
      _onOk.Fire();
    }
  }
}
