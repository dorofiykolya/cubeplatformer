using Game.Messages;
using Injection;

namespace Game.UI.Windows
{
  public class UITutorialWindow : UIWindow<UITutorialWindowComponent, UIWindowData>
  {
    [Inject]
    private GameContext _gameContext;

    protected override void OnInitialize()
    {
      Component.Ok.Subscribe(Lifetime, () =>
      {
        Close();
        _gameContext.Tell(new PlayMessage());
      });
    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {

    }
  }
}
