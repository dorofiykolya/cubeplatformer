using ClassicLogic.Engine;

namespace ClassicLogic.Outputs
{
  public class GuardActionEvent : OutputEvent
  {
    public int GuardId;
    public Action Action;

    public void Set(int id, Action action)
    {
      GuardId = id;
      Action = action;
    }
  }
}
