using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Views.Components;

namespace Game.Logics.ClassicLogic.Commands
{
  public class ClassicLogicInitializeCommand : ClassicLogicCommand<InitializeEvent>
  {
    protected override void Execute(InitializeEvent evt, ClassicLogicEngine engine)
    {
      var level = engine.Level;
      foreach (var guardData in evt.Guard)
      {
        var cell = level[guardData.Position.x, level.Size.Y - 1 - guardData.Position.y, 0];
        var view = cell.Content as CellGuardContentComponent;
        view.transform.SetParent(engine.GuardTransform);
        view.transform.localPosition = level.CoordinateConverter.ToWorld(new PositionF(guardData.Position.x, guardData.Position.y, 0f));
        engine.AddGuard(guardData.Id, view);
      }

      var runner = level[evt.Runner.x, level.Size.Y - 1 - evt.Runner.y, 0].Content as CellPlayerContentComponent;
      runner.transform.SetParent(level.transform);
      runner.transform.localPosition = level.CoordinateConverter.ToWorld(new PositionF(evt.Runner.x, evt.Runner.y, 0f));
      engine.Runner = runner;

      for (int x = 0; x < evt.Map.Length; x++)
      {
        for (int y = 0; y < evt.Map[0].Length; y++)
        {
          var type = evt.Map[x][y];
          if (type == TileType.HLADR_T || type == TileType.TRAP_T)
          {
            level[x, level.Size.Y - 1 - y, 0].Content.gameObject.SetActive(false);
            if (type == TileType.HLADR_T)
            {
              engine.HideLadder = level[x, level.Size.Y - 1 - y, 0].Content;
            }
          }
        }
      }
    }
  }
}
