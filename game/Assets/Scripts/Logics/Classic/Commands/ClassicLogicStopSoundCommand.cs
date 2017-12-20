using ClassicLogic.Outputs;
using Game.Controllers;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicStopSoundCommand : ClassicLogicCommand<StopSoundEvent>
  {
    protected override void Execute(StopSoundEvent evt, ClassicLogicEngine engine)
    {
      engine.Context.Controllers.Get<GameSoundController>().Play(evt.Sound.ToString().ToLower());
    }
  }
}
