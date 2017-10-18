using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicStartFillHoleCommand : ClassicLogicCommand<StartFillHoleEvent>
  {
    protected override void Execute(StartFillHoleEvent evt, ClassicLogicEngine engine)
    {
      engine.Level[evt.X, engine.Level.Size.Y - 1 - evt.Y, 0].Content.gameObject.SetActive(false);
    }
  }
}
