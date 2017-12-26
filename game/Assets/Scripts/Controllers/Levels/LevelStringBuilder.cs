using Game.Logics.Classic;
using Game;
using Game.Components;

namespace Game.Controllers
{
    public class LevelStringBuilder
    {
        public static LevelComponent CreateLevel(string data, CellPreset preset)
        {
            var csv = new CSVLevelReader(data);
            var size = new LevelSize
            {
                X = csv.Width,
                Y = csv.Height,
                Z = 1
            };

            var level = LevelFactory.Create(size, new ClassicLogicLevelCoordinateConverter());

            for (int y = 0; y < csv.Height; y++)
            {
                for (int x = 0; x < csv.Width; x++)
                {
                    var type = csv[x, y];
                    var cellInfo = preset.GetByType(type.Type, type.Direction);
                    cellInfo.Id = type.Id;
                    level[x, y, 1].SetContent(cellInfo);
                }
            }
            return level;
        }
    }
}