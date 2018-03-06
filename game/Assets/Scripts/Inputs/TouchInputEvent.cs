using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Inputs
{
  public class TouchInputEvent
  {
    public class Pool
    {
      private static readonly Stack<TouchInputEvent> Stack = new Stack<TouchInputEvent>();

      public static TouchInputEvent Pop(int id, Vector2 position)
      {
        if (Stack.Count != 0)
        {
          var result = Stack.Pop();
          result._inPool = false;
          result.Initialize(id, position);
          return result;
        }
        return new TouchInputEvent(id, position);
      }

      public static void Release(TouchInputEvent evt)
      {
        if (evt._inPool) throw new InvalidOperationException();
        evt._inPool = true;
        Stack.Push(evt);
      }
    }

    public struct TouchPosition
    {
      public Vector2 Position;
      public float Time;
    }

    private bool _inPool;
    public int Id { get; private set; }
    public int FingerId { get; private set; }
    public Vector2 Position { get { return Positions[Positions.Count - 1].Position; } }
    public TouchPhase Phase { get; private set; }
    public Vector2 DeltaPosition { get; private set; }
    public Vector2 StartPosition { get { return Positions[0].Position; } }
    public float Time { get { return Positions[Positions.Count - 1].Time; } }
    public List<TouchPosition> Positions { get; private set; }
    public TouchPosition First { get { return Positions[0]; } }
    public TouchPosition Last { get { return Positions[Positions.Count - 1]; } }
    public bool Canceled { get; private set; }

    private TouchInputEvent(int id, Vector2 position)
    {
      Positions = new List<TouchPosition>();
      Initialize(id, position);
    }

    private void Initialize(int id, Vector2 position)
    {
      Id = id;
      FingerId = id - 1;
      Positions.Clear();
      Update(position);
      Phase = TouchPhase.Began;
      Canceled = false;
    }

    public void Update(Vector2 position)
    {
      Positions.Add(new TouchPosition { Position = position, Time = Environment.TickCount / 1000f });
      DeltaPosition = Positions.Count > 1 ? position - Positions[Positions.Count - 2].Position : Vector2.zero;
      Phase = TouchPhase.Moved;
    }

    public void End(Vector2 position)
    {
      Update(position);
      Phase = TouchPhase.Ended;
    }

    public void Cancel(Vector2 position)
    {
      Update(position);
      Phase = TouchPhase.Ended;
      Canceled = true;
    }
  }
}
