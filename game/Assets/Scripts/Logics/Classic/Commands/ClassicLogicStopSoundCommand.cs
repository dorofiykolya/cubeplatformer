using ClassicLogic.Outputs;
using Game.Managers;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicStopSoundCommand : ClassicLogicCommand<StopSoundEvent>
  {
    protected override void Execute(StopSoundEvent evt, ClassicLogicEngine engine)
    {
      engine.Context.Managers.Get<GameSoundManager>().Play(evt.Sound.ToString().ToLower());
    }
  }
}
