using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicInitializeCommand : ClassicLogicCommand<InitializeEvent>
  {
    protected override void Execute(InitializeEvent evt, ClassicLogicEngine engine)
    {
      var viewContext = engine.ViewContext;
      
      viewContext.AddTiles(evt.Map);

      foreach (var guardData in evt.Guard)
      {
        viewContext.AddGuard(guardData.Id, guardData.Position.x, guardData.Position.y);
      }

      viewContext.AddRunner(evt.Runner.x, evt.Runner.y);

      viewContext.OptimizeTiles();
    }
  }
}
