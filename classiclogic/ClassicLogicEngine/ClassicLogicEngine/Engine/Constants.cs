using System.Collections.Generic;

namespace ClassicLogic.Engine
{
  public class Constants
  {
    public const double TWidth = 40;
    public const double THeight = 44;

    public const double BASE_TILE_X = 1;//40;
    public const double BASE_TILE_Y = 1;//44;

    public const double W = BASE_TILE_X;
    public const double H = BASE_TILE_Y;

    public const double W2 = W / 2.0;
    public const double H2 = H / 2.0;

    public const double W4 = W / 4.0;
    public const double H4 = H / 4.0;

    public const int NO_OF_TILES_X = 28;
    public const int NO_OF_TILES_Y = 16;

    public const double tileW = W;
    public const double tileH = H;

    public const int maxTileX = NO_OF_TILES_X - 1;
    public const int maxTileY = NO_OF_TILES_Y - 1;

    public const int SCORE_GET_GOLD = 250;
    public const int SCORE_GUARD_DEAD = 75;
    public const int SCORE_IN_HOLE = 75;

    public const int TICK_COUNT_PER_TIME = 16;
    public const int MAX_TIME_COUNT = 999; //for moden mode

    public const int MAX_OLD_GUARD = 6;   //maximum number of guards for AI Version 1 and 2
    public const int MAX_NEW_GUARD = 5;   //for AI Version >= 3

    public static readonly int[] SHAKE_TIME_OLD = { 36, 3, 3, 3, 3, 3 };
    public static readonly int[] SHAKE_TIME_NEW = { 51, 3, 3, 3, 3, 3 };

    public static readonly int[][] MOVE_POLICY_OLD =
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

    public static readonly int[][] MOVE_POLICY_NEW =
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


    public static readonly Dictionary<AIVersion, Configuration> CONFIGURATION = new Dictionary<AIVersion, Configuration>
    {
      { AIVersion.V1, new Configuration
        {
          runnerSpeed = 0.65,
          guardSpeed  = 0.3,
          digSpeed    = 0.68,
          fillSpeed   = 0.24,
          xMoveBase   = 8 / TWidth * BASE_TILE_X,
          yMoveBase   = 8 / THeight * BASE_TILE_Y,

          digitLimit = 6,

          maxGuard = MAX_OLD_GUARD,

          movePolicy = MOVE_POLICY_OLD,

          shakeTime = SHAKE_TIME_OLD
        }},
      { AIVersion.V2, new Configuration
        {
          runnerSpeed = 0.70,
          guardSpeed = 0.35,
          digSpeed = 0.68,
          fillSpeed = 0.27,
          xMoveBase = 8 / TWidth * BASE_TILE_X,
          yMoveBase = 9 / THeight * BASE_TILE_Y,

          digitLimit = 6,

          maxGuard = MAX_OLD_GUARD,

          movePolicy = MOVE_POLICY_OLD,

          shakeTime = SHAKE_TIME_OLD
        }
      },
      { AIVersion.V3, new Configuration
        {
          runnerSpeed = 0.8,
          guardSpeed = 0.4,
          digSpeed = 1,
          fillSpeed = 1,
          xMoveBase = 8 / TWidth * BASE_TILE_X,
          yMoveBase = 9 / THeight * BASE_TILE_Y,

          digitLimit = 6,

          maxGuard = MAX_OLD_GUARD,

          movePolicy = MOVE_POLICY_OLD,

          shakeTime = SHAKE_TIME_OLD
        }
      },
      { AIVersion.V4, new Configuration
        {
          runnerSpeed = 0.8,
          guardSpeed = 0.4,
          digSpeed = 1,
          fillSpeed = 1,
          xMoveBase = 8 / TWidth * BASE_TILE_X,
          yMoveBase = 9 / THeight * BASE_TILE_Y,

          digitLimit = 8,

          maxGuard = MAX_NEW_GUARD,

          movePolicy = MOVE_POLICY_NEW,

          shakeTime = SHAKE_TIME_NEW
        }
      }
    };


  }
}
