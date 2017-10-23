using ClassicLogic.Engine;

namespace ClassicLogic.Outputs
{
  public class InitializeEvent : OutputEvent
  {
    public TileType[][] Map;
    public GuardData[] Guard;
    public Point Runner;

    public Action RunnerAction;
    public Shape RunnerShape;

    public class GuardData
    {
      public int Id;
      public Point Position;

      public Action Action;
      public Shape Shape;
    }

  }
}
