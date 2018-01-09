using UnityEngine;

namespace Game.Components
{
  [RequireComponent(typeof(Player2DControllerComponent))]
  public class Player2DInputControllerComponent : MonoBehaviour
  {
    private Player2DControllerComponent _player2DController;

    public float InputHorizontalThreashold = 0.1f;
    public float InputVerticalThreashold = 0.1f;

    private bool _jump;

    private void Awake()
    {
      _player2DController = GetComponent<Player2DControllerComponent>();
    }

    private void Update()
    {
      if (!_jump)
      {
        _jump = UnityInput.GetAxis(UnityInputAxis.Jump) > float.Epsilon;
      }
    }

    private void FixedUpdate()
    {
      var h = UnityInput.GetAxis(UnityInputAxis.Horizontal);
      if (Mathf.Abs(h) < Mathf.Abs(InputHorizontalThreashold)) h = 0;

      var v = UnityInput.GetAxis(UnityInputAxis.Vertical);
      if (Mathf.Abs(v) < Mathf.Abs(InputVerticalThreashold)) v = 0;

      _player2DController.Move(new Vector2(h, v));

      if (_jump)
      {
        _player2DController.Jump();
        _jump = false;
      }
    }
  }
}
