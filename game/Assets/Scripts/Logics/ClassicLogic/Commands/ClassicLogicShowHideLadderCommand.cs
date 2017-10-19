using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicShowHideLadderCommand : ClassicLogicCommand<ShowHideLadderEvent>
  {
    protected override void Execute(ShowHideLadderEvent evt, ClassicLogicEngine engine)
    {
      engine.ViewContext.ShowHiddenLadder();
    }
  }
}
