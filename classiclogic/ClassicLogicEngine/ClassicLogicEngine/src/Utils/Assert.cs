using System;
namespace ClassicLogic.Utils
{
  public class Assert
  {
    public Assert()
    {
    }

    public static void IsTrue(bool expression, string message = "")
    {
      if (!expression) throw new InvalidOperationException(message);
    }
  }
}
