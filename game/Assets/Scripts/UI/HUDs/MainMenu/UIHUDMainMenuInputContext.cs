using Game;
using Game.Inputs;
using Utils;

namespace Assets.Scripts.UI.HUDs.MainMenu
{
  public class UIHUDMainMenuInputContext : InputContext
  {
    public UIHUDMainMenuInputContext(GameContext context, Lifetime lifetime) : base(context, lifetime)
    {
    }

    public UIHUDMainMenuInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
    {
    }
  }
}
