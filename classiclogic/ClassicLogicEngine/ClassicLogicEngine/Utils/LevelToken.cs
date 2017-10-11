using ClassicLogic.Engine;

namespace ClassicLogic.Utils
{
  public class LevelToken
  {
    private readonly TileType _tileType;

    public LevelToken(TileType tileType)
    {
      _tileType = tileType;
    }

    public TileType Type { get { return _tileType; } }
  }
}
