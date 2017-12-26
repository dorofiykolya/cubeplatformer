namespace Game
{
  public enum CellDirection
  {
    None,
    Left,
    Right,
    Top,
    Bottom
  }

  public static class CellDirectionExtenstion
  {
    public static CellDirection Invert(this CellDirection direction)
    {
      switch (direction)
      {
        case CellDirection.Bottom: return CellDirection.Top;
        case CellDirection.Top: return CellDirection.Bottom;
        case CellDirection.Left: return CellDirection.Right;
        case CellDirection.Right: return CellDirection.Left;
      }
      return CellDirection.None;
    }
  }

}