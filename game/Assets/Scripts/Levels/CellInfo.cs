using System;
using UnityEngine;

namespace Game
{
  [Serializable]
  public struct CellInfo : IEquatable<CellInfo>
  {
    public CellType Type;
    public string Name;
    public GameObject Prefab;

    public static bool operator ==(CellInfo p1, CellInfo p2)
    {
      return p1.Equals(p2);
    }

    public static bool operator !=(CellInfo p1, CellInfo p2)
    {
      return !p1.Equals(p2);
    }

    public bool Equals(CellInfo other)
    {
      return Type == other.Type && Prefab == other.Prefab && (string.Equals(Name, other.Name) || string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(other.Name));
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is CellInfo && Equals((CellInfo)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (int)Type;
        hashCode = (hashCode * 397) ^ (Prefab != null ? Prefab.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
        return hashCode;
      }
    }
  }
}