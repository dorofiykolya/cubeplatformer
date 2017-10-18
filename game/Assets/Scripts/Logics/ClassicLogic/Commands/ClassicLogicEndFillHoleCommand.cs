using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicEndFillHoleCommand : ClassicLogicCommand<EndFillHoleEvent>
  {
    protected override void Execute(EndFillHoleEvent evt, ClassicLogicEngine engine)
    {
      engine.Level[evt.X, engine.Level.Size.Y - 1 - evt.Y, 0].Content.gameObject.SetActive(true);
    }
  }
}
