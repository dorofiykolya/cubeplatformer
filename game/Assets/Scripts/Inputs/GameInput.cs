namespace Game.Inputs
{
  public class GameInput
  {
    public static readonly GameInput Vertical = new GameInput("Vertical");
    public static readonly GameInput Horizontal = new GameInput("Horizontal");
    public static readonly GameInput Action = new GameInput("Action");
    public static readonly GameInput Submit = new GameInput("Submit");
    public static readonly GameInput Cancel = new GameInput("Cancel");

    public GameInput(string name)
    {
      Name = name;
    }

    public string Name { get; private set; }
  }
}