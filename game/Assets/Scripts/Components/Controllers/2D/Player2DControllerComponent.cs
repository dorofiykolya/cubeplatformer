using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Components
{
  public class Player2DControllerComponent : MonoBehaviour
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
      _next = _level.CoordinateConverter.ToPosition(transform.localPosition);
    }

    private void Update()
    {
      var h = Input.GetAxis("Horizontal");
      var moveTo = _moveTo;
      if (h > 0) moveTo = MoveTo.Right;
      else if (h < 0) moveTo = MoveTo.Left;

      var position = _level.CoordinateConverter.ToPosition(transform.localPosition);
      if (moveTo != _moveTo)
      {
        _next = position;
        _moveTo = moveTo;
        if (_moveTo == MoveTo.Right)
        {
          _next = position.GetPosition(CellDirection.Right);
        }
        else if (_moveTo == MoveTo.Left)
        {
          _next = position.GetPosition(CellDirection.Left);
        }
      }

      var nextVector = _level.CoordinateConverter.ToWorld(_next + new PositionF(0.5f, 0, 0));
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextVector, Speed * Time.deltaTime);
      if (_next == position)
      {
        _moveTo = MoveTo.None;
      }
    }
  }
}
