using System;
using System.Collections.Generic;
using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using Game.Components;
using UnityEngine;
using Utils;
using Point = Utils.Point;

namespace Game.Logics.Classic
{
  public class ClassicLogicViewContext
  {
    private readonly Dictionary<CellType, CellInfo> _cache = new Dictionary<CellType, CellInfo>();
    private readonly Dictionary<int, CellGuardContentComponent> _guards = new Dictionary<int, CellGuardContentComponent>();
    private readonly Dictionary<Point, CellContentComponent> _tiles = new Dictionary<Point, CellContentComponent>();
    private readonly Dictionary<Point, CellContentComponent> _golds = new Dictionary<Point, CellContentComponent>();

    private Transform _rootTransform;
    private Transform _guardTransform;
    private Transform _tileTransform;
    private Transform _goldTransform;
    private ClassicLogicCoordinateConverter _converter;

    public ClassicLogicViewContext(Lifetime lifetime, int maxTileX, int maxTileY)
    {
      _rootTransform = new GameObject(GetType().Name).transform;

      _guardTransform = new GameObject("Guards").transform;
      _guardTransform.SetParent(_rootTransform, false);

      _tileTransform = new GameObject("Tiles").transform;
      _tileTransform.SetParent(_rootTransform, false);

      _goldTransform = new GameObject("Golds").transform;
      _goldTransform.SetParent(_rootTransform, false);

      _converter = new ClassicLogicCoordinateConverter(maxTileX, maxTileY);

      lifetime.AddAction(() =>
      {
        _cache.Clear();
        if (_rootTransform != null)
        {
          var go = _rootTransform.gameObject;
          _rootTransform = null;
          GameObject.Destroy(go);
        }
      });
    }

    public CellPlayerContentComponent Runner { get; set; }
    public CellContentComponent HideLadder { get; set; }
    public CellPreset Preset { get; set; }

    public CellGuardContentComponent GetGuard(int guardId)
    {
      return _guards[guardId];
    }

    public void AddGuard(int id, int x, int y)
    {
      var cellType = CellType.Guard;
      CellInfo cellInfo;
      if (!_cache.TryGetValue(cellType, out cellInfo))
      {
        _cache[cellType] = cellInfo = Preset.GetByType(cellType);
      }
      if (cellInfo.Prefab != null)
      {
        var view = GameObject.Instantiate(cellInfo.Prefab) as CellGuardContentComponent;
        view.transform.SetParent(_guardTransform);
        _guards[id] = view;
        view.transform.localPosition = _converter.ToWorld(new PositionF(x + 0.5f, y, 0));
      }
    }

    public void AddTile(TileType type, int x, int y)
    {
      var cellType = ClassicLogicTypeConverter.Convert(type);
      CellInfo cellInfo;
      if (!_cache.TryGetValue(cellType, out cellInfo))
      {
        _cache[cellType] = cellInfo = Preset.GetByType(cellType);
      }
      if (cellType != CellType.Guard && cellType != CellType.Player)
      {
        if (cellInfo.Prefab != null)
        {
          if (cellType == CellType.Coin)
          {
            AddGold(x, y);
          }
          else
          {
            var position = new Point(x, y);
            var view = GameObject.Instantiate(cellInfo.Prefab);
            _tiles[position] = view;
            view.transform.SetParent(_tileTransform, false);


            view.transform.localPosition = _converter.ToWorld(new PositionF(x, y, 0));

            if (cellType == CellType.HideLadder)
            {
              view.gameObject.SetActive(false);
              HideLadder = view;
            }
            else if (cellType == CellType.Trap)
            {
              view.gameObject.SetActive(false);
            }
          }
        }
      }
    }

    public void AddRunner(int x, int y)
    {
      var cellType = CellType.Player;
      CellInfo cellInfo;
      if (!_cache.TryGetValue(cellType, out cellInfo))
      {
        _cache[cellType] = cellInfo = Preset.GetByType(cellType);
      }
      if (cellInfo.Prefab != null)
      {
        var position = new Point(x, y);
        var view = GameObject.Instantiate(cellInfo.Prefab) as CellPlayerContentComponent;
        view.transform.SetParent(_rootTransform);
        Runner = view;
        view.gameObject.tag = "Player";
        view.transform.localPosition = _converter.ToWorld(new PositionF(x + 0.5f, y, 0));
      }
    }

    public void AddGold(int x, int y)
    {
      var view = GameObject.Instantiate(_cache[CellType.Coin].Prefab);
      _golds[new Point(x, y)] = view;
      view.transform.SetParent(_goldTransform, false);
      view.transform.localPosition = _converter.ToWorld(new PositionF(x, y, 0));
    }

    public void RemoveGold(int x, int y)
    {
      var position = new Point(x, y);
      var view = _golds[position];
      _golds.Remove(position);
      GameObject.Destroy(view.gameObject);
    }

    public void EndFillHole(int x, int y)
    {
      _tiles[new Point(x, y)].gameObject.SetActive(true);
    }

    public void StartFillHole(int x, int y)
    {
      _tiles[new Point(x, y)].gameObject.SetActive(false);
    }

    public void MoveGuard(int id, double x, double y)
    {
      GetGuard(id).transform.localPosition = _converter.ToWorld(new PositionF((float)x + 0.5f, (float)y, 0));
    }

    public void MoveRunner(double x, double y)
    {
      Runner.transform.localPosition = _converter.ToWorld(new PositionF((float)x + 0.5f, (float)y, 0));
    }

    public void RunnerDead()
    {
      Runner.gameObject.SetActive(false);
    }

    public void ShowHiddenLadder()
    {
      if (HideLadder)
      {
        HideLadder.gameObject.SetActive(true);
      }
    }

    public void ShowTrap(int x, int y)
    {
      _tiles[new Point(x, y)].gameObject.SetActive(true);
    }

    public void StopDigging()
    {

    }

    public void DiggingComplete()
    {

    }

    public void DiggingProgress(double ratio)
    {

    }

    public void StartDigging()
    {

    }
  }
}

