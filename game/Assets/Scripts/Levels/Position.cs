using System;

namespace Game
{
  [Serializable]
  public struct PositionF
  {
    public float X;
    public float Y;
    public float Z;

    public PositionF(float x, float y, float z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public static PositionF operator +(PositionF left, PositionF right)
    {
      return new PositionF(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static PositionF operator -(PositionF left, PositionF right)
    {
      return new PositionF(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public static PositionF operator *(PositionF left, PositionF right)
    {
      return new PositionF(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    public static PositionF operator /(PositionF left, PositionF right)
    {
      return new PositionF(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }

    public static implicit operator Position(PositionF point)
    {
      return new Position((int)point.X, (int)point.Y, (int)point.Z);
    }

    public static bool operator ==(PositionF s1, PositionF s2)
    {
      return s1.X == s2.X && s1.Y == s2.Y && s1.Z == s2.Z;
    }

    public static bool operator !=(PositionF s1, PositionF s2)
    {
      return s1.X != s2.X || s1.Y != s2.Y || s1.Z != s2.Z;
    }

    public bool Equals(PositionF other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is PositionF && Equals((PositionF)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (int)X;
        hashCode = (hashCode * 397) ^ (int)Y;
        hashCode = (hashCode * 397) ^ (int)Z;
        return hashCode;
      }
    }

    public override string ToString()
    {
      return string.Format("X:{0}, Y:{1}, Z:{2}", X, Y, Z);
    }

    public PositionF Set(int x, int y, int z)
    {
      X = x;
      Y = y;
      Z = z;
      return this;
    }
  }

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

    public Position(int x, int y)
    {
      X = x;
      Y = y;
      Z = 0;
    }

    public static implicit operator PositionF(Position point)
    {
      return new PositionF(point.X, point.Y, point.Z);
    }

    public static bool operator ==(Position s1, Position s2)
    {
      return s1.X == s2.X && s1.Y == s2.Y && s1.Z == s2.Z;
    }

    public static bool operator !=(Position s1, Position s2)
    {
      return s1.X != s2.X || s1.Y != s2.Y || s1.Z != s2.Z;
    }

    public bool Equals(Position other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Position && Equals((Position)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = X;
        hashCode = (hashCode * 397) ^ Y;
        hashCode = (hashCode * 397) ^ Z;
        return hashCode;
      }
    }

    public override string ToString()
    {
      return string.Format("X:{0}, Y:{1}, Z:{2}", X, Y, Z);
    }

    public Position Set(int x, int y, int z)
    {
      X = x;
      Y = y;
      Z = z;
      return this;
    }
  }
}