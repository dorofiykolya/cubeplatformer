using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils;

namespace Game.Components
{
  public class Player2DControllerComponent : MonoBehaviour
  {
    private enum MoveTo
    {
      None,
      Left,
      Right,
      Up,
      Down
    }


    public float Speed = 5f;
    public bool SnapToCell = false;
    public Transform HeadCollision;
    public Transform GroundCollision;

    public Vector2 Next;

    private LevelComponent _level;
    private MoveTo _moveTo;
    private Position _next;

    private void Awake()
    {
      _level = GetComponentInParent<LevelComponent>();
      _next = _level.CoordinateConverter.ToPosition(transform.localPosition);
    }

    /*
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
    }*/

    private float _horizontalInput;

    private void Update()
    {
      
    }

    private void FixedUpdate()
    {
      var body = GetComponent<Rigidbody2D>();
      var h = Input.GetAxis("Horizontal");
      var moveTo = _moveTo;
      var isMove = true;
      if (h > 0.1f) moveTo = MoveTo.Right;
      else if (h < -0.1f) moveTo = MoveTo.Left;
      else isMove = false;

      var positionF = _level.CoordinateConverter.ToPositionF(transform.localPosition);
      var position = _level.CoordinateConverter.ToPosition(transform.localPosition);
      var offset = positionF - (PositionF)position;
      if (isMove)
      {
        _next = position;
        _moveTo = moveTo;
        if (_moveTo == MoveTo.Right)
        {
          if (offset.X < 0.3f)
          {
            _next = position;
          }
          else
          {
            _next = position.GetPosition(CellDirection.Right);
          }
        }
        else if (_moveTo == MoveTo.Left)
        {
          if (offset.X > 0.7f)
          {
            _next = position;
          }
          else
          {
            _next = position.GetPosition(CellDirection.Left);
          }
        }
      }

      var size = _level.CoordinateConverter.ToWorld(new PositionF(1, 0, 0));
      var nextVector = (Vector2)_level.CoordinateConverter.ToWorld(_next + new PositionF(0.5f, 0, 0));
      Next = nextVector;
      var lastVelocity = body.velocity;
      if (isMove)
      {
        body.velocity = new Vector2(Speed * h * size.x, lastVelocity.y);
      }
      else if(SnapToCell)
      {
        var currentVelocity = body.velocity.x;
        var targetVelocity = (nextVector.x - transform.localPosition.x) * Speed;
        //var velocity = currentVelocity;
        //var resultVelocity = Mathf.SmoothDamp(currentVelocity, targetVelocity, ref velocity, Time.fixedDeltaTime, Speed * size.x, Time.fixedDeltaTime);
        body.velocity = new Vector2(targetVelocity, lastVelocity.y); 
      }
      if (_next == position)
      {
        _moveTo = MoveTo.None;
      }
    }
  }
}
