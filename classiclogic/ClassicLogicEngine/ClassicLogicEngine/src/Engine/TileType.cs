using System;
namespace ClassicLogic.Engine
{
  //value | Character | Type
  //------+-----------+-----------
  //  0x0 |  <space>  | Empty space
  //  0x1 |     #     | Normal Brick
  //  0x2 |     @     | Solid Brick
  //  0x3 |     H     | Ladder
  //  0x4 |     -     | Hand-to-hand bar (Line of rope)
  //  0x5 |     X     | False brick
  //  0x6 |     S     | Ladder appears at end of level
  //  0x7 |     $     | Gold chest
  //  0x8 |     0     | Guard
  //  0x9 |     &     | Player  

  public enum TileType
  {
    EMPTY_T = 0x00,
    BLOCK_T = 0x01,
    SOLID_T = 0x02,
    LADDR_T = 0x03,
    BAR_T = 0x04,
    TRAP_T = 0x05,
    HLADR_T = 0x06,
    GOLD_T = 0x07,
    GUARD_T = 0x08,
    RUNNER_T = 0x09,

    REBORN_T = 0x10 //template: for reborn
  }
}
