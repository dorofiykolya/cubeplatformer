using System;
using Game.Views.Components;

namespace Game.Logics.Classics
{
  public class LogicMapModule : ILogicModule
  {
    private LogicModules _logicModules;
    private LogicCell[,] _grid;

    public LogicMapModule(LogicModules logicModules)
    {
      this._logicModules = logicModules;
    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      
    }

    public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      
    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {
      
    }

    internal void Initialize(LevelSize size, CellComponent[] grid)
    {
      _grid = new LogicCell[size.X, size.Y];
      foreach (var cell in grid)
      {
        _grid[cell.Position.X, cell.Position.Y] = new LogicCell(cell.CellType, cell.Position.X, cell.Position.Y);
        if(cell.CellType == CellType.Runner)
        {
            _logicModules.Get<LogicRunnerModule>().AddRunner(cell);
        }
        else if(cell.CellType == CellType.Guard)
        {
            _logicModules.Get<LogicGuardModule>().AddGuard(cell);
        }
      }
    }
  }
}