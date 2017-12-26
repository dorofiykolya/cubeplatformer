using System.Text;
using Game.Components;

namespace Game.Logics.Classic
{
  public class ClassicLogicLevelConverter
  {
    public static string Convert(LevelComponent level)
    {
      var str = new StringBuilder();

      for (int y = 0; y < level.Size.Y; y++)
      {
        for (int x = 0; x < level.Size.X; x++)
        {
          switch (level[x, level.Size.Y - y - 1, 0].CellType)
          {
            case CellType.Block:
              str.Append('#');
              break;
            case CellType.Solid:
              str.Append('@');
              break;
            case CellType.Ladder:
              str.Append('H');
              break;
            case CellType.Rope:
              str.Append('-');
              break;
            case CellType.Trap:
              str.Append('X');
              break;
            case CellType.HLadr:
              str.Append('S');
              break;
            case CellType.Gold:
              str.Append('$');
              break;
            case CellType.Guard:
              str.Append('0');
              break;
            case CellType.Player:
              str.Append('&');
              break;
            case CellType.Empty:
              str.Append(' ');
              break;
          }
        }
        str.Append('\n');
      }

      return str.ToString();
    }
  }
}
