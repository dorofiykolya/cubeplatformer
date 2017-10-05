using ClassicLogic.Engine;

namespace ClassicLogic.Outputs
{
  public class InitializeEvent : OutputEvent
  {
    public TileType[][] Map;
    public GuardData[] Guard;
    public Point Runner;

    public class GuardData
    {
      public int Id;
      public Point Position;
    }

  }
}
