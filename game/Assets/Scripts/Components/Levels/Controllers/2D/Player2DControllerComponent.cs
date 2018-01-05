using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(Rigidbody2D))]
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

    [Tooltip("max move speed")]
    public float Speed = 5f;
    public bool SnapToCell = false;
    [Range(0, 1), Tooltip("offset - snap to cell")]
    public float Offset = 0.3f;

    [Header("Collisions")]
    public Transform HeadCollision;
    public Transform GroundCollision;

    [Header("Debug")]
    public Vector2 Next;

    private LevelComponent _level;
    private MoveTo _moveTo;
    private Position _next;

    private Vector2 _inputMove;
    private Rigidbody2D _body;
    private ILevelCoordinateConverter _coordinateConverter;
    private Vector3 _unitSize;
    private bool _contextInput;

    public void Move(Vector2 inputMove)
    {
      if (!_contextInput)
      {
        _inputMove = inputMove;
      }
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
  }
}
