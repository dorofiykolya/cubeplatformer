namespace Game.Inputs
{
  public struct InputEvent
  {
    public GameInput Input;
    public float Value;
    public InputPhase Phase;
    public InputController Controller;
    public InputUpdate Update;
  }
}