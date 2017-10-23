using ClassicLogic.Outputs;

namespace Game.Logics.Classic.Commands
{
  public class ClassicLogicInitializeCommand : ClassicLogicCommand<InitializeEvent>
  {
    protected override void Execute(InitializeEvent evt, ClassicLogicEngine engine)
    {
      var viewContext = engine.ViewContext;

      for (int x = 0; x < evt.Map.Length; x++)
      {
        for (int y = 0; y < evt.Map[0].Length; y++)
        {
          var type = evt.Map[x][y];
          viewContext.AddTile(type, x, y);
        }
      }

      foreach (var guardData in evt.Guard)
      {
        viewContext.AddGuard(guardData.Id, guardData.Position.x, guardData.Position.y);
      }

      viewContext.AddRunner(evt.Runner.x, evt.Runner.y);
    }
  }
}
