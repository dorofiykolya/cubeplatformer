using ClassicLogic.Engine;

namespace ClassicLogic.Outputs
{
  public class GuardShapeEvent : OutputEvent
  {
    public int GuardId;
    public Shape Shape;

    public void Set(int id, Shape shape)
    {
      GuardId = id;
      Shape = shape;
    }
  }
}
