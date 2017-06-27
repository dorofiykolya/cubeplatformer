using Utils;

namespace Game
{
  public class InputManager : IInputManager
  {
    private readonly Lifetime _lifetime;

    public InputManager(Lifetime lifetime)
    {
      _lifetime = lifetime;
    }
  }
}