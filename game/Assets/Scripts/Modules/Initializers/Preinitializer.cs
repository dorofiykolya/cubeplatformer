using Utils;

namespace Game
{
  public static class PreInitializer<T>
  {
    public static void Preinitialize(T obj)
    {
      MethodInvoker<T, PreinitializeAttribute>.Invoke(obj);
    }
  }
}