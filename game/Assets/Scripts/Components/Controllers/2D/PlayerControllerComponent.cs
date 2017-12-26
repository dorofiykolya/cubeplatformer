using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Components
{
  public class PlayerControllerComponent : MonoBehaviour
  {
    private enum MoveTo
    {
      None,
      Left,
      Right
    }


    public float Speed = 5f;

    private LevelComponent _level;
    private MoveTo _moveTo;
    private Position _next;

    private void Awake()
    {
      _level = GetComponentInParent<LevelComponent>();
    }

    private void Update()
    {
      var h = Input.GetAxis("Horizontal");
      var moveTo = MoveTo.None;
      if (h > 0) moveTo = MoveTo.Right;
      else if (h < 0) moveTo = MoveTo.Left;

      var position = _level.CoordinateConverter.ToPosition(transform.localPosition);
      if (moveTo != _moveTo)
      {
        _next = position;

        if (_moveTo == MoveTo.Right)
        {
          _next = position.GetPosition(CellDirection.Right);
        }
        else if (_moveTo == MoveTo.Left)
        {
          _next = position.GetPosition(CellDirection.Left);
        }
      }
      
      if (_next != position)
      {
        var nextVector = _level.CoordinateConverter.ToWorld(_next);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextVector, Speed * Time.deltaTime);
      }
      else
      {
        _moveTo = MoveTo.None;
      }
    }
  }
}
