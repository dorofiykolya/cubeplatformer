namespace ClassicLogic.Engine
{
  public class Configuration
  {
    //speed
    public double runnerSpeed;
    public double guardSpeed;
    public double digSpeed;
    public double fillSpeed;
    public double xMoveBase;
    public double yMoveBase;

    public int maxGuard;

    public int digitLimit;

    public int[][] movePolicy;
    public int[] shakeTime;

    public SpriteSheet createHole()
    {
      return new SpriteSheet
      {
        {
          Shape.digHoleLeft, new SheetAnimation
          {
            frames = new int[]{0,1,2,3,4,5,6,7},
            next = null,
            speed = digSpeed
          }
        },
        {
          Shape.digHoleRight, new SheetAnimation
          {
            frames = new []{8,9,10,11,12,13,14,15},
            next = null,
            speed = digSpeed
          }
        },
        {
          Shape.fillHole,new SheetAnimation
          {
            frames = new []{16, 16, 16, 16, 16, 16, 16, 16, 16,
                 16, 16, 16, 16, 16, 16, 16, 16, 16,
                 16, 16, 16, 16, 16, 16, 16, 16, 16,
                 16, 16, 16, 16, 16, 16, 16, 16, 16,
                 16, 16, 16, 16, 16, 16, 16, 16, 16,
                 17, 17, 18, 18, 19},
            next = null,
            speed = fillSpeed
          }
        }
      };
    }

    public SpriteSheet createRunner()
    {
      return new SpriteSheet
      {
        {
          Shape.runRight, new SheetAnimation
          {
            frames = new[] {0, 1, 2},
            next = Shape.runRight,
            speed = runnerSpeed
          }
        },
        {
          Shape.runLeft, new SheetAnimation
          {
            frames = new[] {3, 4, 5},
            next = Shape.runLeft,
            speed = runnerSpeed
          }
        },
        {
          Shape.runUpDn, new SheetAnimation
          {
            frames = new[] {6, 7},
            next = Shape.runUpDn,
            speed = runnerSpeed
          }
        },
        {
          Shape.barRight, new SheetAnimation
          {
            frames = new[] {18, 19, 19, 20, 20},
            next = Shape.barRight,
            speed = runnerSpeed
          }
        },
        {
          Shape.barLeft, new SheetAnimation
          {
            frames = new[] {21, 22, 22, 23, 23},
            next = Shape.barLeft,
            speed = runnerSpeed
          }
        },
        {
          Shape.digRight, new SheetAnimation
          {
            frames = new[] {24}
          }
        },
        {
          Shape.digLeft, new SheetAnimation
          {
            frames = new[] {25}
          }
        },
        {
          Shape.fallRight, new SheetAnimation
          {
            frames = new[] {8}
          }
        },
        {
          Shape.fallLeft, new SheetAnimation
          {
            frames = new[] {26}
          }
        }
      };
    }

    public SpriteSheet createGuard()
    {
      return new SpriteSheet
      {
        {
          Shape.runRight, new SheetAnimation
          {
            frames = new[] {0, 1, 2},
            next = Shape.runRight,
            speed = guardSpeed
          }
        },
        {
          Shape.runLeft, new SheetAnimation
          {
            frames = new[] {3, 4, 5},
            next = Shape.runLeft,
            speed = guardSpeed
          }
        },
        {
          Shape.runUpDn, new SheetAnimation
          {
            frames = new[] {6, 7},
            next = Shape.runUpDn,
            speed = guardSpeed
          }
        },
        {
          Shape.barLeft, new SheetAnimation
          {
            frames = new[] {25, 26, 26, 27, 27},
            next = Shape.barLeft,
            speed = guardSpeed
          }
        },
        {
          Shape.barRight, new SheetAnimation
          {
            frames = new[] {22, 23, 23, 24, 24},
            next = Shape.barRight,
            speed = guardSpeed
          }
        },
        {
          Shape.reborn, new SheetAnimation
          {
            frames = new[] {28, 28, 29},
            speed = guardSpeed
          }
        },
        {
          Shape.fallRight, new SheetAnimation
          {
            frames = new[] {8}
          }
        },
        {
          Shape.fallLeft, new SheetAnimation
          {
            frames = new[] {30}
          }
        },
        {
          Shape.shakeRight, new SheetAnimation
          {
            frames = new int[]
            {
              8, 8, 8, 8, 8, 8, 8,
              8, 8, 8, 8, 8, 8,
              9, 10, 9, 10, 8
            },
            next = null,
            speed = guardSpeed
          }
        },
        {
          Shape.shakeLeft, new SheetAnimation
          {
            frames = new[]
            {
              30, 30, 30, 30, 30, 30, 30,
              30, 30, 30, 30, 30, 30,
              31, 32, 31, 32, 30
            },
            next = null,
            speed = guardSpeed
          }
        }
      };
    }
  }
}

