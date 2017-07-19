using System;
using Game.Views.Components;

namespace Game.Logics.Classics
{
  public class LogicGridModule : ILogicModule
  {
    private LogicModules _logicModules;
    private LogicCell[,] _grid;
    public int curAiVersion = 3;


    public LogicGridModule(LogicModules logicModules)
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

    public LogicCell this[int x, int y]
    {
      get { return _grid[x, y]; }
    }

    public float TileX { get { return 1; } }
    public float TileY { get { return 1; } }

    public int MaxTileX { get { return _grid.GetLength(0) - 1; } }
    public int MaxTileY { get { return _grid.GetLength(1) - 1; } }

    public float W2 { get { return 1f / 2f; } }
    public float W4 { get { return 1f / 4f; } }

    public float H2 { get { return 1f / 2f; } }
    public float H4 { get { return 1f / 4f; } }
    public float TileH { get { return 1f; } }
    public float TileW { get { return 1f; } }

    internal void Initialize(LevelSize size, CellComponent[] grid)
    {
      _grid = new LogicCell[size.X, size.Y];
      foreach (var cell in grid)
      {
        _grid[cell.Position.X, cell.Position.Y] = new LogicCell(cell.CellType, cell.Position.X, cell.Position.Y);
        if (cell.CellType == CellType.Player)
        {
          _logicModules.Get<LogicPlayerModule>().AddPlayer(cell);
        }
        else if (cell.CellType == CellType.Guard)
        {
          var id = _logicModules.Get<LogicGuardModule>().AddGuard(cell);
          _logicModules.Get<LogicViewModule>().AddGuard(id, cell);
        }
      }
    }
  }
}