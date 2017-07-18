namespace Game.Inputs
{
  public class InputController
  {
    public InputController(string name, int id)
    {
      this.Name = name;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
  }
}