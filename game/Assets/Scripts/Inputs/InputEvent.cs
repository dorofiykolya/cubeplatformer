namespace Game.Inputs
{
  public struct InputEvent
  {
    public GameInput Input;
    public float Value;
    public InputPhase Phase;
    public int ControllerId;
    public InputUpdate Update;
  }
}