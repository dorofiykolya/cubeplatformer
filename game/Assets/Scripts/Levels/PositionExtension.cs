using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
  public static class PositionExtension
  {
    public static Position GetPosition(this Position position, CellDirection direction)
    {
      switch (direction)
      {
        case CellDirection.Bottom: return new Position(position.X, position.Y + 1);
        case CellDirection.Top: return new Position(position.X, position.Y - 1);
        case CellDirection.Left: return new Position(position.X - 1, position.Y);
        case CellDirection.Right: return new Position(position.X + 1, position.Y);
      }
      return position;
    }
  }
}
