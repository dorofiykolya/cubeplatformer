using System;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class CellPreset : ScriptableObject
  {
    public string Name;
    public CellInfo[] Cells;
    public CellInfo GetByType(CellType type)
    {
      foreach (var item in Cells)
      {
          if(item.Type == type)
          {
            return item;
          }
      }
      return new CellInfo();
    }

    public CellInfo GetByType(CellType type, CellDirection direction)
    {
      foreach (var item in Cells)
      {
          if(item.Type == type && item.Direction == direction)
          {
            return item;
          }
      }
      return GetByType(type);
    }
  }
}