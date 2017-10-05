using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClassicLogic.Engine;
using Point = ClassicLogic.Engine.Point;

namespace ClassicLogicEngineViewer
{
  public class TileContainer
  {
    private readonly Dictionary<int, Guard> _guards = new Dictionary<int, Guard>();
    private readonly Dictionary<Point, Tile> _map = new Dictionary<Point, Tile>();
    private readonly Canvas _canvas;
    private readonly Canvas _topCanvas;

    private Digging _digging;

    private Runner _runner;

    public TileContainer(Canvas canvas, Canvas topCanvas)
    {
      _canvas = canvas;
      _topCanvas = topCanvas;
    }

    public IEnumerable<Tile> Tiles { get { return _map.Values; } }
    public Runner Runner { get { return _runner; } }

    public Tile AddTile(int x, int y, TileType type)
    {
      var tile = new Tile();
      tile.Type = type;
      Canvas.SetLeft(tile, x * ViewConstants.TileWidth);
      Canvas.SetTop(tile, y * ViewConstants.TileHeight);
      _map.Add(new Point
      {
        x = x,
        y = y
      }, tile);
      _canvas.Children.Add(tile);
      return tile;
    }

    public void RemoveTile(int x, int y)
    {
      var position = new Point
      {
        x = x,
        y = y
      };
      var tile = _map[position];
      _canvas.Children.Remove(tile);
      _map.Remove(position);
    }

    public Guard AddGuard(Point position, int id)
    {
      var guard = new Guard();
      guard.HasGold = Visibility.Hidden;
      guard.Id = id;
      guard.Position = position;
      _guards.Add(id, guard);
      Canvas.SetLeft(guard, position.x * ViewConstants.TileWidth);
      Canvas.SetTop(guard, position.y * ViewConstants.TileHeight);
      _topCanvas.Children.Add(guard);
      return guard;
    }

    public Runner AddRunner(Point position)
    {
      _runner = new Runner();
      _topCanvas.Children.Add(_runner);
      _runner.Position = position;
      Canvas.SetLeft(_runner, position.x * ViewConstants.TileWidth);
      Canvas.SetTop(_runner, position.y * ViewConstants.TileHeight);
      return _runner;
    }

    public void MoveRunner(double x, double y)
    {
      _runner.Position = new Point
      {
        x = (int)x,
        y = (int)y
      };
      Canvas.SetLeft(_runner, x * ViewConstants.TileWidth);
      Canvas.SetTop(_runner, y * ViewConstants.TileHeight);
    }

    public void MoveGuard(int guardId, double x, double y)
    {
      var guard = _guards[guardId];
      guard.Position = new Point
      {
        x = (int)x,
        y = (int)y
      };
      Canvas.SetLeft(guard, x * ViewConstants.TileWidth);
      Canvas.SetTop(guard, y * ViewConstants.TileHeight);
    }

    public void GuardHasGold(int guardId, bool hasGold)
    {
      _guards[guardId].HasGold = hasGold ? Visibility.Visible : Visibility.Hidden;
    }

    public void AddGold(int x, int y)
    {
      RemoveTile(x, y);
      AddTile(x, y, TileType.GOLD_T);
    }

    public void RemoveGold(int x, int y)
    {
      RemoveTile(x, y);
      AddTile(x, y, TileType.EMPTY_T);
    }

    public void StartFillHole(int x, int y)
    {
      _map[new Point(x, y)].Opacity = 0.3;
    }

    public void EndFillHole(int x, int y)
    {
      _map[new Point(x, y)].Opacity = 1.0;
    }

    public void ShowTrap(int x, int y)
    {
      _map[new Point(x, y)].Visibility = Visibility.Visible;
    }

    public void StartDigging(int x, int y)
    {
      _digging = new Digging();
      _topCanvas.Children.Add(_digging);
      Canvas.SetLeft(_digging, x * ViewConstants.TileWidth);
      Canvas.SetTop(_digging, y * ViewConstants.TileHeight);
    }

    public void StopDigging()
    {
      _topCanvas.Children.Remove(_digging);
    }

    public void DiggingComplete()
    {
      _topCanvas.Children.Remove(_digging);
    }

    public void ProcessDigging(double ratio)
    {
      _digging.Ratio = ratio;
    }

    public void ProcessFillHole(int x, int y, double ratio)
    {
      _map[new Point(x, y)].Opacity = 0.3 + (ratio * 0.6);
    }
  }
}
