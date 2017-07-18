using Utils.Collections;

namespace Game.Logics.Classics
{
  public class LogicActions
  {
    private readonly PriorityQueueComparable<ILogicAction> _queue = new PriorityQueueComparable<ILogicAction>();

    public int Count
    {
      get { return _queue.Count; }
    }

    public ILogicAction Peek()
    {
      return _queue.Peek();
    }

    public ILogicAction Dequeue()
    {
      return _queue.Dequeue();
    }

    public void Enqueue(ILogicAction action)
    {
      _queue.Enqueue(action);
    }
  }
}