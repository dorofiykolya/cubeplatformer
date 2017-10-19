using System;
using ClassicLogic.Engine;

namespace Game.Logics.ClassicLogic
{
  public class ClassicLogicTypeConverter
  {
    public static TileType Convert(CellType type)
    {
      switch (type)
      {
        case CellType.Rope: return TileType.BAR_T;
        case CellType.Block: return TileType.BLOCK_T;
        case CellType.Empty: return TileType.EMPTY_T;
        case CellType.Gold: return TileType.GOLD_T;
        case CellType.Guard: return TileType.GUARD_T;
        case CellType.HLadr: return TileType.HLADR_T;
        case CellType.Ladder: return TileType.LADDR_T;
        case CellType.Reborn: return TileType.REBORN_T;
        case CellType.Player: return TileType.RUNNER_T;
        case CellType.Solid: return TileType.SOLID_T;
        case CellType.Trap: return TileType.TRAP_T;
      }
      throw new ArgumentException();
    }

    public static CellType Convert(TileType type)
    {
      switch (type)
      {
        case TileType.BAR_T: return CellType.Rope;
        case TileType.BLOCK_T: return CellType.Block;
        case TileType.EMPTY_T: return CellType.Empty;
        case TileType.GOLD_T: return CellType.Gold;
        case TileType.GUARD_T: return CellType.Guard;
        case TileType.HLADR_T: return CellType.HLadr;
        case TileType.LADDR_T: return CellType.Ladder;
        case TileType.REBORN_T: return CellType.Reborn;
        case TileType.RUNNER_T: return CellType.Player;
        case TileType.SOLID_T: return CellType.Solid;
        case TileType.TRAP_T: return CellType.Trap;
      }
      throw new ArgumentException();
    }
  }
}
