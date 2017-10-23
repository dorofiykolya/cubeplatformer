using System.Collections.Generic;

namespace ClassicLogic.Engine
{
  public class SpriteSheet : Dictionary<Shape, SheetAnimation>
  {

  }

  public class SheetAnimation
  {
    public int[] frames;
    public Shape? next;
    public double speed;
  }
}
