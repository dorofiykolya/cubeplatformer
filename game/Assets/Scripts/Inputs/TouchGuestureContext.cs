using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils;

namespace Game.Inputs
{
  public class TouchGuestureContext
  {
    public static float MinSwipeDistance = 30f;
    public static float MaxClickTime = 1.5f;
    public static float MaxClickPositionThreashold = 10f;

    private readonly InputContext _context;
    private readonly Lifetime _lifetime;

    public TouchGuestureContext(InputContext context, Lifetime lifetime)
    {
      _context = context;
      _lifetime = lifetime;
    }

    public void SubscribeClick(Lifetime lifetime, Rect percentScreen, Action listener)
    {
      _context.SubscribeTouch(lifetime, TouchPhase.Began, evt =>
      {
        if (EventSystemWrapper.IsPointerOverGameObject(evt.FingerId)) return;

        var x = evt.Position.x / Screen.width;
        var y = evt.Position.y / Screen.height;
        if (!percentScreen.Contains(new Vector2(x, y))) return;

        _context.SubscribeTouch(lifetime, evt.Id, TouchPhase.Ended, endEvt =>
        {
          if (EventSystemWrapper.IsPointerOverGameObject(endEvt.FingerId)) return;
          var time = endEvt.Last.Time - endEvt.StartTime;
          if (time <= MaxClickTime)
          {
            var start = endEvt.StartPosition;
            if (endEvt.Positions.All(p => (p.Position - start).sqrMagnitude < Mathf.Sqrt(MaxClickPositionThreashold)))
            {
              listener();
            }
          }
        });
      });
    }

    public void SubscribeSwipe(Lifetime lifetime, SwipeDirection direction, Action listener)
    {
      _context.SubscribeTouch(lifetime, TouchPhase.Ended, evt =>
      {
        var delta = evt.Last.Position - evt.First.Position;
        var deltaTime = evt.Last.Time - evt.First.Time;
        if (deltaTime < 1)
        {
          if (delta.sqrMagnitude >= Mathf.Sqrt(MinSwipeDistance))
          {
            var dir = delta.normalized;
            float angle = Vector2.Angle(dir, Vector2.right);

            if (angle <= 45.0f) // right
            {
              if (direction == SwipeDirection.Right) listener();
            }
            else if (angle >= 135.0f) //left
            {
              if (direction == SwipeDirection.Left) listener();
            }
            else if (dir.y > 0.0f) // top
            {
              if (direction == SwipeDirection.Up) listener();
            }
            else // bottom
            {
              if (direction == SwipeDirection.Down) listener();
            }
          }
        }
      });
    }

    private class Guesture
    {

    }
  }
}
