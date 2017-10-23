using System.Collections.Generic;

namespace ClassicLogic.Engine
{
  public class Constants
  {
    public const double Width = 40;
    public const double Height = 44;

    public const double BaseTileX = 1;//40;
    public const double BaseTileY = 1;//44;

    public const double W = BaseTileX;
    public const double H = BaseTileY;

    public const double W2 = W / 2.0;
    public const double H2 = H / 2.0;

    public const double W4 = W / 4.0;
    public const double H4 = H / 4.0;

    public const double TileW = W;
    public const double TileH = H;

    public const int ScoreGetGold = 250;
    public const int ScoreGuardDead = 75;
    public const int ScoreInHole = 75;

    public const int TickCountPerTime = 16;
    public const int MaxTimeCount = 999; //for moden mode

    public const int MaxOldGuard = 6;   //maximum number of guards for AI Version 1 and 2
    public const int MaxNewGuard = 5;   //for AI Version >= 3

    public static readonly int[] ShakeTimeOld = { 36, 3, 3, 3, 3, 3 };
    public static readonly int[] ShakeTimeNew = { 51, 3, 3, 3, 3, 3 };

    public static readonly int[][] MovePolicyOld =
    {
      new[] {0, 0, 0, 0, 0, 0},
      new[] {0, 1, 1, 0, 1, 1}, // change
      new[] {1, 1, 1, 1, 1, 1},
      new[] {1, 2, 1, 1, 2, 1},
      new[] {1, 2, 2, 1, 2, 2},
      new[] {2, 2, 2, 2, 2, 2},
      new[] {2, 2, 3, 2, 2, 3},
      new[] {2, 3, 3, 2, 3, 3},
      new[] {3, 3, 3, 3, 3, 3},
      new[] {3, 3, 4, 3, 3, 4},
      new[] {3, 4, 4, 3, 4, 4},
      new[] {4, 4, 4, 4, 4, 4}
    };

    public static readonly int[][] MovePolicyNew =
    {
      new[] {0, 0, 0, 0, 0, 0},
      new[] {0, 1, 1, 0, 1, 1}, // change
      new[] {1, 1, 1, 1, 1, 1},
      new[] {1, 2, 1, 1, 2, 1},
      new[] {1, 2, 2, 1, 2, 2},
      new[] {2, 2, 2, 2, 2, 2},
      new[] {2, 2, 3, 2, 2, 3},
      new[] {2, 3, 3, 2, 3, 3},
      new[] {3, 3, 3, 3, 3, 3},
      new[] {3, 3, 4, 3, 3, 4},
      new[] {3, 4, 4, 3, 4, 4},
      new[] {4, 4, 4, 4, 4, 4}
    };


    public static readonly Dictionary<AiVersion, Configuration> Configuration = new Dictionary<AiVersion, Configuration>
    {
      { AiVersion.V1, new Configuration
        {
          RunnerSpeed = 0.65,
          GuardSpeed  = 0.3,
          DigSpeed    = 0.68,
          FillSpeed   = 0.24,
          XMoveBase   = 8 / Width * BaseTileX,
          YMoveBase   = 8 / Height * BaseTileY,

          DigitLimit = 6,

          MaxGuard = MaxOldGuard,

          MovePolicy = MovePolicyOld,

          ShakeTime = ShakeTimeOld
        }},
      { AiVersion.V2, new Configuration
        {
          RunnerSpeed = 0.70,
          GuardSpeed = 0.35,
          DigSpeed = 0.68,
          FillSpeed = 0.27,
          XMoveBase = 8 / Width * BaseTileX,
          YMoveBase = 9 / Height * BaseTileY,

          DigitLimit = 6,

          MaxGuard = MaxOldGuard,

          MovePolicy = MovePolicyOld,

          ShakeTime = ShakeTimeOld
        }
      },
      { AiVersion.V3, new Configuration
        {
          RunnerSpeed = 0.8,
          GuardSpeed = 0.4,
          DigSpeed = 1,
          FillSpeed = 1,
          XMoveBase = 8 / Width * BaseTileX,
          YMoveBase = 9 / Height * BaseTileY,

          DigitLimit = 6,

          MaxGuard = MaxOldGuard,

          MovePolicy = MovePolicyOld,

          ShakeTime = ShakeTimeOld
        }
      },
      { AiVersion.V4, new Configuration
        {
          RunnerSpeed = 0.8,
          GuardSpeed = 0.4,
          DigSpeed = 1,
          FillSpeed = 1,
          XMoveBase = 8 / Width * BaseTileX,
          YMoveBase = 9 / Height * BaseTileY,

          DigitLimit = 8,

          MaxGuard = MaxNewGuard,

          MovePolicy = MovePolicyNew,

          ShakeTime = ShakeTimeNew
        }
      }
    };


  }
}
