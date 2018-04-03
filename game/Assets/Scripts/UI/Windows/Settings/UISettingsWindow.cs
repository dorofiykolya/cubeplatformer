using Game.Inputs;
using Injection;
using Utils;

namespace Game.UI.Windows
{
  public class UISettingsWindow : UIWindow<UISettingsWindowComponent, UIWindowData>
  {
    [Inject]
    private GameContext _gameContext;

    private SettingsInputContext _input;

    protected override void OnInitialize()
    {

    }

    protected override void OnOpen()
    {
      _input = new SettingsInputContext(_gameContext, Lifetime, _gameContext.InputContext.Current);
      _input.Subscribe(Lifetime, GameInput.Cancel, InputPhase.End, InputUpdate.Update, evt => Close());

      Component.SubscribeOnClose(Lifetime, Close);
    }

    protected override void OnClose()
    {

    }

    private class SettingsInputContext : InputContext
    {
      public SettingsInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
      {
      }
    }
  }
}
