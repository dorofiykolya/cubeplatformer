namespace Game.Logics.Classics
{
    public class LogicCell
    {
        public CellType Base;
        public CellType Act;

        public Position Position;

        public LogicCell(CellType cellType, int x, int y)
        {
            Position = new Position(x, y, 0);

            switch (cellType)
            {
                case CellType.Empty:
                    Base = Act = CellType.Empty;
                    break;
                case CellType.Block:
                    Base = Act = CellType.Block;
                    break;
                case CellType.Solid:
                    Base = Act = CellType.Solid;
                    break;
                case CellType.Ladder:
                    Base = Act = CellType.Ladder;
                    break;
                case CellType.Rope:
                    Base = Act = CellType.Rope;
                    break;
                case CellType.HLadr:
                    Base = CellType.HLadr;
                    Act = CellType.Empty;
                    break;
                case CellType.Gold:
                    Base = CellType.Gold;
                    Act = CellType.Empty;
                    break;
                case CellType.Guard:
                    Base = CellType.Empty;
                    Act = CellType.Guard;
                    break;
                case CellType.Player:
                    Base = CellType.Empty;
                    Act = CellType.Player;
                    break;
            }
        }
    }
}