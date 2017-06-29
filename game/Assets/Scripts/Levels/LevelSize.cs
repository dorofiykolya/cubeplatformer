using System;

namespace Game
{
  [Serializable]
  public struct LevelSize
  {
    public int X;
    public int Y;
    public int Z;

    public override string ToString()
    {
      return string.Format("X:{0}, Y:{1}, Z:{2}", X, Y, Z);
    }
  }
}