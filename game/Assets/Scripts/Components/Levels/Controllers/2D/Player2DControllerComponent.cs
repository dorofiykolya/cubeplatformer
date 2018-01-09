using System;
using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class Player2DControllerComponent : MonoBehaviour, IMovable
  {
    private enum MoveTo
    {
      None,
      Left,
      Right,
      Up,
      Down
    }

    private static Collider2D[] _collisions = new Collider2D[20];

    [Tooltip("max move speed")]
    public float Speed = 5f;
    public bool SnapToCell = false;
    [Range(0, 1), Tooltip("offset - snap to cell")]
    public float Offset = 0.3f;

    public float JumpThreashold = 0.1f;
    public float JumpForce = 10f;
    public bool AirControl = false;

    public CharacterAnimatorController CharacterAnimatorController;

    [Header("Collisions")]
    public Transform HeadCollision;
    public float HeadCollisionRadius = 0.5f;
    public Transform GroundCollision;
    public float GroundCollisionRadius = 0.5f;

    [Header("Debug")]
    public Vector2 Next;

    private LevelComponent _level;
    private MoveTo _moveTo;
    private Position _next;

    private Vector2 _inputMove;
    private bool _inputJump;
    private Rigidbody2D _body;
    private ILevelCoordinateConverter _coordinateConverter;
    private Vector3 _unitSize;
    private bool _contextInput;
    private bool _onMovable;
    private bool _onCanJump;

    public void Move(Vector2 inputMove)
    {
      if (!_contextInput)
      {
        _inputMove = inputMove;
      }
    }

    public void Jump()
    {
      _inputJump = true;
    }

    private void Awake()
    {
      _body = GetComponent<Rigidbody2D>();
      _level = GetComponentInParent<LevelComponent>();
      _coordinateConverter = _level.CoordinateConverter;
      _unitSize = _coordinateConverter.ToWorld(new PositionF(1, 0, 0));
      _next = _coordinateConverter.ToPosition(transform.localPosition);

      var controller = _level.GetComponent<LevelControllerComponent>();
      if (controller != null && controller.Context != null)
      {
        _contextInput = true;
      }
    }

    private void FixedUpdate()
    {
      var moveTo = _moveTo;
      var verticalMove = MoveTo.None;
      var isMove = true;
      if (_inputMove.x > float.Epsilon)
      {
        moveTo = MoveTo.Right;
      }
      else if (_inputMove.x < -float.Epsilon)
      {
        moveTo = MoveTo.Left;
      }
      else
      {
        isMove = false;
      }
      if (_inputMove.y > float.Epsilon)
      {
        verticalMove = MoveTo.Up;
      }
      else if (_inputMove.y < -float.Epsilon)
      {
        verticalMove = MoveTo.Down;
      }
      else
      {
        verticalMove = MoveTo.None;
      }

      var crouch = false;

      var filter = new ContactFilter2D();
      int collisions = 0;
      if ((collisions = Physics2D.OverlapCircle(HeadCollision.position, HeadCollisionRadius, filter, _collisions)) != 0)
      {
        for (int i = 0; i < collisions; i++)
        {
          var movable = _collisions[i].GetComponent<MovableMaterialComponent>();
          if (movable)
          {
            crouch = true;
            break;
          }
        }
      }

      _onMovable = false;
      _onCanJump = false;

      if ((collisions = Physics2D.OverlapCircle(GroundCollision.position, GroundCollisionRadius, filter, _collisions)) != 0)
      {
        for (int i = 0; i < collisions; i++)
        {
          var movable = _collisions[i].GetComponent<MovableMaterialComponent>();
          if (movable)
          {
            _onMovable = true;
            if (movable.CanJump(this))
            {
              _onCanJump = true;
              break;
            }
          }
        }
      }

      if (_inputJump && _onMovable && _onCanJump && Math.Abs(_body.velocity.y) < JumpThreashold)
      {
        _onMovable = false;
        _onCanJump = false;
        _body.AddForce(new Vector2(0, JumpForce));
      }
      _inputJump = false;

      if (!AirControl && !_onMovable)
      {
        isMove = false;
      }

      var localPosition = transform.localPosition;

      var positionF = _coordinateConverter.ToPositionF(localPosition);
      var position = _coordinateConverter.ToPosition(localPosition);
      var offset = positionF - (PositionF)position;
      if (isMove)
      {
        _next = position;
        _moveTo = moveTo;
        if (_moveTo == MoveTo.Right)
        {
          if (offset.X < Offset)
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
          if (offset.X > (1 - Offset))
          {
            _next = position;
          }
          else
          {
            _next = position.GetPosition(CellDirection.Left);
          }
        }
      }

      var nextVector = (Vector2)_coordinateConverter.ToWorld(_next + new PositionF(0.5f, 0, 0));
      Next = nextVector;
      var lastVelocity = _body.velocity;
      if (isMove)
      {
        _body.velocity = new Vector2(Speed * _inputMove.x * _unitSize.x, lastVelocity.y);
      }
      else if (SnapToCell)
      {
        var targetVelocity = (nextVector.x - localPosition.x) * Speed;
        _body.velocity = new Vector2(targetVelocity, lastVelocity.y);
      }
      if (_next == position)
      {
        _moveTo = MoveTo.None;
      }
    }

    private void OnDrawGizmos()
    {
      Gizmos.DrawWireSphere(HeadCollision.position, HeadCollisionRadius);
      Gizmos.DrawWireSphere(GroundCollision.position, GroundCollisionRadius);
    }


  }
}
