namespace Game.Components.Controllers
{
  public class Movable2DMaterialComponent : MovableMaterialComponent
  {
    public bool Jump = true;

    public override bool CanJump(IMovable movable)
    {
      return Jump && base.CanJump(movable);
    }
  }
}
