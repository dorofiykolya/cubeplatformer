namespace ClassicLogic.Engine
{
  public class Tile
  {
    public TileType Act;
    public TileType Base;
    public Action Action;
    public int Index;
    public int X;
    public int Y;

    public Point ToPoint()
    {
      return new Point(X, Y);
    }
  }
}
