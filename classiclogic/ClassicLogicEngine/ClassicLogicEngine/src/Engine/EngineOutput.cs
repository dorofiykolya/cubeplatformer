using System;
using System.Collections.Generic;
using ClassicLogic.Outputs;

namespace ClassicLogic.Engine
{
  public class EngineOutput
  {
    private Dictionary<Type, Stack<OutputEvent>> _pool = new Dictionary<Type, Stack<OutputEvent>>();
    private readonly Queue<OutputEvent> _queue = new Queue<OutputEvent>();

    public int Count
    {
      get { return _queue.Count; }
    }

    public OutputEvent Dequeue()
    {
      return _queue.Dequeue();
    }

    public T Enqueue<T>(int tick) where T : OutputEvent, new()
    {
      T result;
      Stack<OutputEvent> stack;
      if (_pool.TryGetValue(typeof(T), out stack) && stack.Count != 0)
      {
        result = (T)stack.Pop();
      }
      else
      {
        result = new T();
      }
      result.Tick = tick;
      _queue.Enqueue(result);
      return result;
    }

    public void Return(OutputEvent evt)
    {
      Stack<OutputEvent> stack;
      if (!_pool.TryGetValue(evt.GetType(), out stack))
      {
        _pool[evt.GetType()] = stack = new Stack<OutputEvent>();
      }
      stack.Push(evt);
    }
  }
}
