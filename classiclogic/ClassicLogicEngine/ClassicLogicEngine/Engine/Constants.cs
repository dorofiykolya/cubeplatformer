using System;
using System.Collections.Generic;
namespace ClassicLogic.Engine
{
  public class Constants
  {
    public const int NO_OF_TILES_X = 28;
    public const int NO_OF_TILES_Y = 16;

    public static int MAX_OLD_GUARD = 6;   //maximum number of guards for AI Version 1 and 2
    public static int MAX_NEW_GUARD = 5;   //for AI Version >= 3

    public static readonly Dictionary<AIVersion, Configuration> CONFIGURATION = new Dictionary<AIVersion, Configuration>
    {
      { AIVersion.V1, new Configuration
        {
          runnerSpeed = 0.65,
          guardSpeed  = 0.3,
          digSpeed    = 0.68,
          fillSpeed   = 0.24,
          xMoveBase   = 8,
          yMoveBase   = 8,

          maxGuard = MAX_OLD_GUARD
        }},
      { AIVersion.V2, new Configuration 
        { 
          runnerSpeed = 0.70, 
          guardSpeed = 0.35, 
          digSpeed = 0.68, 
          fillSpeed = 0.27, 
          xMoveBase = 8, 
          yMoveBase = 9,

          maxGuard = MAX_OLD_GUARD
        }
      },
      { AIVersion.V3, new Configuration 
        { 
          runnerSpeed = 0.8,  
          guardSpeed = 0.4,  
          digSpeed = 1,    
          fillSpeed = 1,    
          xMoveBase = 8, 
          yMoveBase = 9,

          maxGuard = MAX_OLD_GUARD
        }
      },
      { AIVersion.V4, new Configuration
        {
          runnerSpeed = 0.8,
          guardSpeed = 0.4,
          digSpeed = 1,
          fillSpeed = 1,
          xMoveBase = 8,
          yMoveBase = 9,

          maxGuard = MAX_NEW_GUARD
        }
      }
    };


  }
}
