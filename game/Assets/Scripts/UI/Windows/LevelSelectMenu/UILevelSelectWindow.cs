using Game.Inputs;
using Injection;
using Utils;

namespace Game.UI.Windows
{
  public class UILevelSelectWindow : UIWindow<UILevelSelectWindowComponent, UILevelSelectWindowData>
  {
    [Inject]
    private GameContext _gameContext;

    private UIInputContext _input;

    protected override void OnInitialize()
    {

    }

    protected override void OnOpen()
    {
      _input = new UIInputContext(_gameContext, Lifetime, _gameContext.InputContext.Current);
      _input.Subscribe(Lifetime, GameInput.Cancel, InputPhase.End, InputUpdate.Update, evt => Close());

      Component.SubscribeOnClose(Lifetime, Close);

      Component.SetTitle("Level:" + Data.Level);
    }

    protected override void OnClose()
    {

    }

    private class UIInputContext : InputContext
    {
      public UIInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
      {
      }
    }
  }
}
