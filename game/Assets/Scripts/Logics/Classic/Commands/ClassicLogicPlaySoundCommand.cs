using ClassicLogic.Outputs;
using Game.Controllers;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicPlaySoundCommand : ClassicLogicCommand<PlaySoundEvent>
  {
    protected override void Execute(PlaySoundEvent evt, ClassicLogicEngine engine)
    {
      engine.Context.Controllers.Get<GameSoundController>().Play(evt.Sound.ToString().ToLower());
    }
  }
}
