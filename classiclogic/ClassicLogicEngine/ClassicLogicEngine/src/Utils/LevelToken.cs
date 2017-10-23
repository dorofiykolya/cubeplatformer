using ClassicLogic.Engine;

namespace ClassicLogic.Utils
{
  public class LevelToken
  {
    private readonly TileType _tileType;
    private readonly LevelTokenType _tokenType;

    public LevelToken(LevelTokenType tokenType)
    {
      _tokenType = tokenType;
    }

    public LevelToken(TileType tileType) : this(LevelTokenType.TileType)
    {
      _tileType = tileType;
    }

    public TileType Type { get { return _tileType; } }
    public LevelTokenType TokenType { get { return _tokenType; } }
  }

  public enum LevelTokenType
  {
    TileType,
    EndLine
  }
}
