using Utils;

namespace Game
{
  public static class Initializer<T>
  {
    public static void Initialize(T obj)
    {
      MethodInvoker<T, InitializeAttribute>.Invoke(obj);
    }
  }
}