namespace Game.Components.Controllers
{
  public class Climb2DMaterialComponent : ClimbMaterialComponent
  {
    public bool Jump = true;

    public override bool CanJump(IMovable movable)
    {
      return Jump && base.CanJump(movable);
    }
  }
}
