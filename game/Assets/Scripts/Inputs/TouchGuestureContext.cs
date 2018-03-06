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
    public static float MinSwipeDistance = 25f * (Screen.dpi / 72);
    public static float MaxSwipeTime = 1f;
    public static float MaxClickTime = 2f;
    public static float MaxClickPositionThreashold = 15f * (Screen.dpi / 72);

    private readonly InputContext _context;
    private readonly Lifetime _lifetime;

    public TouchGuestureContext(InputContext context, Lifetime lifetime)
    {
      _context = context;
      _lifetime = lifetime;
    }

    public void SubscribeClick(Lifetime lifetime, Rect percentScreen, Action listener)
    {
      _context.SubscribeTouch(lifetime, TouchPhase.Began, beginEvent =>
      {
        if (EventSystemWrapper.IsPointerOverGameObject(beginEvent.FingerId)) return;

        var x = beginEvent.Position.x / Screen.width;
        var y = beginEvent.Position.y / Screen.height;
        if (!percentScreen.Contains(new Vector2(x, y))) return;

        _context.SubscribeTouch(lifetime, beginEvent.Id, TouchPhase.Ended, endEvent =>
        {
          if (EventSystemWrapper.IsPointerOverGameObject(endEvent.FingerId)) return;
          var time = endEvent.Last.Time - endEvent.First.Time;
          if (time <= MaxClickTime)
          {
            var start = endEvent.StartPosition;
            var maxThreashold = MaxClickPositionThreashold;
            if (endEvent.Positions.All(p => (p.Position - start).magnitude <= maxThreashold))
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
        if (deltaTime <= MaxSwipeTime)
        {
          if (delta.magnitude >= MinSwipeDistance)
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
