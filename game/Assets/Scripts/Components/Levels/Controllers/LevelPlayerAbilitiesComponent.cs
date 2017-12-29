using UnityEngine;

namespace Game.Components
{
  public class LevelPlayerAbilitiesComponent : LevelControllerBaseComponent
  {
    [SerializeField]
    private bool _canMoveLeft = true;
    [SerializeField]
    private bool _canMoveRight = true;
    [SerializeField]
    private bool _canMoveUp = true;
    [SerializeField]
    private bool _canMoveDown = true;
    [SerializeField]
    private bool _canJump = true;
    [SerializeField]
    private bool _canDigg = true;

    public bool CanMoveLeft { get { return _canMoveLeft; } }
    public bool CanMoveRight { get { return _canMoveRight; } }
    public bool CanMoveUp { get { return _canMoveUp; } }
    public bool CanMoveDown { get { return _canMoveDown; } }
    public bool CanJump { get { return _canJump; } }
    public bool CanDigg { get { return _canDigg; } }
  }
}
