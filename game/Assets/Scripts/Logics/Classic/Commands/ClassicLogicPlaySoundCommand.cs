using ClassicLogic.Outputs;
using Game.Managers;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicPlaySoundCommand : ClassicLogicCommand<PlaySoundEvent>
  {
    protected override void Execute(PlaySoundEvent evt, ClassicLogicEngine engine)
    {
      engine.Context.Managers.Get<GameSoundManager>().Play(evt.Sound.ToString().ToLower());
    }
  }
}
