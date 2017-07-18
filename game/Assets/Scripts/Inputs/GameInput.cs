namespace Game.Inputs
{
  public class GameInput
  {
    static GameInput()
    {
      foreach (var field in GameInputField.Inputs)
      {
        field.Input.Name = field.Name;
      }
    }

    public string Name { get; private set; }
    public string GetById(int id)
    {
      return Name + id;
    }

    /// 

    public static readonly GameInput MoveUp = new GameInput();
    public static readonly GameInput MoveDown = new GameInput();
    public static readonly GameInput MoveLeft = new GameInput();
    public static readonly GameInput MoveRight = new GameInput();

    public static readonly GameInput DigLeft = new GameInput();
    public static readonly GameInput DigRight = new GameInput();

    public static readonly GameInput Apply = new GameInput();
    public static readonly GameInput Cancel = new GameInput();
  }
}