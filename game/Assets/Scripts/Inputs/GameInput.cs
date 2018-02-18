namespace Game.Inputs
{
  public class GameInput : IInputName
  {
    // INPUT AXIES
    public static readonly GameInput Vertical = new GameInput("Vertical");
    public static readonly GameInput Horizontal = new GameInput("Horizontal");
    public static readonly GameInput Action = new GameInput("Action");
    public static readonly GameInput Submit = new GameInput("Submit");
    public static readonly GameInput Cancel = new GameInput("Cancel");


    //VIRTUAL PARENT AXIES
    public static readonly GameInput Left = new GameInput("Left", Horizontal, ValueType.Low);
    public static readonly GameInput Right = new GameInput("Right", Horizontal, ValueType.Hight);
    public static readonly GameInput Up = new GameInput("Up", Vertical, ValueType.Hight);
    public static readonly GameInput Down = new GameInput("Down", Vertical, ValueType.Low);
    public static readonly GameInput DigLeft = new GameInput("DigLeft", Action, ValueType.Low);
    public static readonly GameInput DigRight = new GameInput("DigRight", Action, ValueType.Hight);

    public GameInput(string name, GameInput parent = null, ValueType value = ValueType.Both, InputValue inputValue = InputValue.Input)
    {
      Name = name;
      Parent = parent;
      Value = value;
      Input = inputValue;
    }

    public GameInput Parent { get; private set; }
    public string Name { get; private set; }
    public ValueType Value { get; private set; }
    public InputValue Input { get; private set; }

    public enum InputValue
    {
      Input,
      Raw
    }

    public enum ValueType
    {
      Both,
      Hight,
      Low
    }
  }
}