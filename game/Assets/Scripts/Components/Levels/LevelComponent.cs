using UnityEngine;

namespace Game.Components
{
  [ExecuteInEditMode]
  public class LevelComponent : MonoBehaviour
  {
    [SerializeField]
    private Transform _cellContainer;
    [SerializeField]
    private Transform _environmentContainer;
    [SerializeField]
    private LevelSize _size;
    [SerializeField]
    private CellComponent[] _grid;

    public ILevelCoordinateConverter CoordinateConverter
    {
      get
      {
        return GetComponent<LevelCoordinateConverterProviderComponent>().Converter;
      }
    }
    public Transform CellContainer { get { return _cellContainer ?? (_cellContainer = CreateCellContainer()); } }
    public Transform EnviromentContainer
    {
      get { return _environmentContainer ?? (_environmentContainer = CreateEnvironmentContainer()); }
    }

    public LevelSize Size { get { return _size; } }
    public CellComponent[] Grid { get { return _grid; } }
    public LevelLogicComponent Logic { get { return GetComponent<LevelLogicComponent>(); } }

    public CellComponent this[int x, int y, int z]
    {
      get
      {
        var index = (Size.X * Size.Y * z) + x * Size.Y + y;
        if (index >= 0 && index < _grid.Length)
        {
          return _grid[index];
        }
        return null;
      }
    }

    public CellComponent Get(Position position)
    {
      return this[position.X, position.Y, position.Z];
    }

    public CellType GetCellType(Position position)
    {
      var cell = Get(position);
      if (cell != null)
      {
        return cell.CellType;
      }
      return CellType.Empty;
    }

    public void SetContent(LevelSize size, CellComponent[] grid)
    {
      _size = size;
      _grid = grid;
      foreach (var component in _grid)
      {
        component.transform.SetParent(CellContainer);
      }
      UpdateConverter();
    }

    public void UpdateConverter()
    {
      foreach (var component in _grid)
      {
        component.UpdatePosition();
      }
    }

    private Transform CreateCellContainer()
    {
      var cell = transform.Find("Grid");
      if (!cell)
      {
        cell = new GameObject("Grid").transform;
        cell.SetParent(transform, false);
      }
      return cell;
    }

    private Transform CreateEnvironmentContainer()
    {
      var cell = transform.Find("Environment");
      if (!cell)
      {
        cell = new GameObject("Environment").transform;
        cell.SetParent(transform, false);
      }
      return cell;
    }

    private void Awake()
    {
      _cellContainer = CreateCellContainer();
    }
  }
}