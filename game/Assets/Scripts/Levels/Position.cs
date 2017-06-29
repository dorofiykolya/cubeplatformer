using System;

namespace Game
{
  [Serializable]
  public struct Position
  {
    public int X;
    public int Y;
    public int Z;

    public Position(int x, int y, int z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public override string ToString()
    {
      return string.Format("X:{0}, Y:{1}, Z:{2}", X, Y, Z);
    }
  }
}