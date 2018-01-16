using ClassicLogic.Engine;

namespace ClassicLogic.Utils
{
  public class StringLevelReader : LevelReader
  {
    private readonly string _levelData;
    private int _index;
    private LevelToken _token;

    public StringLevelReader(string levelData)
    {
      _levelData = levelData;
      _index = -1;
    }

    public override bool MoveNext()
    {
      _index++;
      if (_index < _levelData.Length)
      {
        switch (_levelData[_index])
        {
          case '#': //Normal Brick
            _token = new LevelToken(TileType.BLOCK_T);
            break;
          case '@': //Solid Brick
            _token = new LevelToken(TileType.SOLID_T);
            break;
          case 'H': //Ladder
            _token = new LevelToken(TileType.LADDR_T);
            break;
          case '-': //Line of rope
            _token = new LevelToken(TileType.BAR_T);
            break;
          case 'X': //False brick
            _token = new LevelToken(TileType.TRAP_T);
            break;
          case 'S': //Ladder appears at end of level
            _token = new LevelToken(TileType.HLADR_T);
            break;
          case '$': //Gold chest
            _token = new LevelToken(TileType.GOLD_T);
            break;
          case '0': //Guard
            _token = new LevelToken(TileType.GUARD_T);
            break;
          case '&': //Player
            _token = new LevelToken(TileType.RUNNER_T);
            break;
          case 'F':
            _token = new LevelToken(TileType.FINISH_T);
            break;
          case ' ': //empty
            _token = new LevelToken(TileType.EMPTY_T);
            break;
          case '\n':
            _token = new LevelToken(LevelTokenType.EndLine);
            break;
          default:
            throw new System.ArgumentException("invalid token");
        }
        return true;
      }
      return false;
    }

    public override LevelToken Token
    {
      get { return _token; }
    }

    public override void Reset()
    {
      _index = -1;
    }
  }
}
