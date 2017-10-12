using System.Text;
using ClassicLogic.Engine;
using ClassicLogic.Utils;
using Game.Views.Components;

namespace Game.Logics.ClassicLogic
{
  public class ClassicLogicEngine : ILogicEngine
  {
    private readonly Engine _engine;

    public ClassicLogicEngine(LevelComponent level)
    {
      var str = new StringBuilder();

      for (int y = 0; y < level.Size.Y; y++)
      {
        for (int x = 0; x < level.Size.X; x++)
        {
          switch (level[x, y, 0].CellType)
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

      _engine = new Engine(AIVersion.V4, new StringLevelReader(str.ToString()), Mode.Modern);
    }

    public void SetAction(Actions.InputAction action)
    {
      _engine.SetAction((InputAction)(int)action);
    }

    public EngineOutput Output
    {
      get { return _engine.Output; }
    }

    public void AddAction(ILogicAction action)
    {

    }

    public void FastForward(int tick)
    {

    }

    public int Tick { get; private set; }
    public bool IsFinished { get; private set; }
    public int MaxTicks { get; private set; }
    public int TicksPerSeconds { get; private set; }
  }
}
