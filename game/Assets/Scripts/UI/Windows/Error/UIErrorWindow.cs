using Game.Inputs;
using Injection;
using Utils;

namespace Game.UI.Windows
{
  public class UIErrorWindow : UIWindow<UIErrorWindowComponent, UIErrorWindowData>
  {
    [Inject]
    private GameContext _gameContext;

    private ErrorInputContext _input;

    protected override void OnInitialize()
    {

    }

    protected override void OnOpen()
    {
      _input = new ErrorInputContext(_gameContext, Lifetime, _gameContext.InputContext.Current);
      _input.Subscribe(Lifetime, GameInput.Cancel, InputPhase.End, InputUpdate.Update, evt => Close());

      Component.SetTitle(Data.Title);
      Component.SetMessage(Data.Message);
    }

    protected override void OnClose()
    {

    }

    private class ErrorInputContext : InputContext
    {
      public ErrorInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
      {
      }
    }
  }
}
