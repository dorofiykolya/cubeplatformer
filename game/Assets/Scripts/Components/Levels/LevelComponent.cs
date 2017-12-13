using UnityEngine;

namespace Game.Views.Components
{
  [ExecuteInEditMode]
  public class LevelComponent : MonoBehaviour
  {
    [SerializeField]
    private Transform _cellContainer;
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
    public LevelSize Size { get { return _size; } }
    public CellComponent[] Grid { get { return _grid; } }
    public LevelLogicComponent Logic { get { return GetComponent<LevelLogicComponent>(); } }

    public CellComponent this[int x, int y, int z]
    {
      get
      {
        var index = (Size.X * Size.Y * z) + x * Size.Y + y;
        return _grid[index];
      }
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

    private void Awake()
    {
      _cellContainer = CreateCellContainer();
    }
  }
}