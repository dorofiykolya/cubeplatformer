using System;

namespace ClassicLogic.Engine
{
  public struct Point : IEquatable<Point>
  {
    public int x;
    public int y;

    public Point(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public bool Equals(Point other)
    {
      return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Point && Equals((Point) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (x * 397) ^ y;
      }
    }

    public override string ToString()
    {
      return string.Format("({0}:{1})", x, y);
    }
  }
}
