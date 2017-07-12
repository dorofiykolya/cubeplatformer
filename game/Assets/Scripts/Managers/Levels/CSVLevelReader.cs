using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class CSVLevelReader
    {
        private int _width;
        private int _height;
        private Dictionary<Point, Cell> _cells;

        public CSVLevelReader(string data, char itemDelim = ';', char lineDelim = '\n')
        {
            _cells = new Dictionary<Point, Cell>();
            var lines = data.Split(lineDelim);

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                var cells = line.Split(itemDelim);
                if (cells.Length > _width) _width = cells.Length;
                for (int x = 0; x < cells.Length; x++)
                {
                    _cells[new Point(x, y)] = Parse(cells[x].Trim());
                }
            }
        }

        public Cell this[int x, int y]
        {
            get
            {
                Cell result;
                _cells.TryGetValue(new Point(x, y), out result);
                return result;
            }
        }

        public int Width { get { return _width; } }

        public int Height { get { return _height; } }

        private Cell Parse(string cell)
        {
            var result = new Cell();
            var value = cell[0];
            var direction = CellDirection.None;
            string id = null;

            // TYPE
            var type = CellType.Empty;
            switch (value)
            {
                case '#': type = CellType.Block; break;
                case 'X': type = CellType.Solid; break;
                case ' ': type = CellType.Empty; break;
                case '$': type = CellType.Gold; break;
                case '@': type = CellType.Player; break;
                case '0': type = CellType.Guard; break;
                case 'H': type = CellType.Ladder; break;
                case '-': type = CellType.Rope; break;
                case 'T': type = CellType.Trap; break;
                case 'S': type = CellType.HLadr; break;
                case 'E': type = CellType.TeleportEnter; break;
                case 'R': type = CellType.TeleportExit; break;
            }

            // ID
            if (cell.Length > 1)
            {
                var startId = cell.IndexOf("{");
                var endId = cell.IndexOf("}");
                if (startId != -1 && endId != -1 && startId < endId)
                {
                    id = new string(cell.Skip(startId).Take(endId - startId).ToArray());
                }
            }

            // DIRECTION
            if (cell.Length > 1)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    cell = cell.Replace(id, "");
                }
                cell = cell.Remove(0, 1);
                var dir = cell[0];
                switch (dir)
                {
                    case '<': direction = CellDirection.Left; break;
                    case '>': direction = CellDirection.Right; break;
                }
            }

            result.Type = type;
            result.Direction = direction;
            result.Id = id;

            return result;
        }

        public struct Cell
        {
            public CellType Type;
            public CellDirection Direction;
            public string Id;
        }
    }
}