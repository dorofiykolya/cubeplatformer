using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.Components.Levels
{
  public class LevelEnvironmentComponent : MonoBehaviour
  {
    public static readonly Point CellSize = new Point(3, 3);

    [SerializeField]
    private LevelEnvironmentPreset _preset;

    [SerializeField]
    private LevelComponent _levelComponent;

    [SerializeField]
    private LevelEnviromentItemContainerComponent[] _items;

    public LevelComponent LevelComponent
    {
      get { return _levelComponent ?? (_levelComponent = GetComponent<LevelComponent>()); }
    }

    public void Build()
    {
      if (_preset == null)
      {
        throw new InvalidOperationException("set preset please");
      }

      foreach (var component in _items)
      {
        DestroyImmediate(component.gameObject);
      }

      var levelComponent = LevelComponent;

      var width = Mathf.CeilToInt(levelComponent.Size.X / (float)CellSize.x);
      var height = Mathf.CeilToInt(levelComponent.Size.Y / (float)CellSize.y);

      var availableTypes = new CellType[]
      {
        CellType.Undefined,
        CellType.Empty,
        CellType.Guard,
        CellType.GuardSpawnPoint,
        CellType.Coin,
        CellType.Player,
      };
      var size = levelComponent.Size;
      var containers = new List<LevelEnviromentItemContainerComponent>();
      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          var available = new List<LevelEnvironmentItemComponent>();

          foreach (var item in _preset.Items)
          {
            var isOk = true;
            for (int ix = 0; ix < CellSize.x && isOk; ix++)
            {
              for (int iy = 0; iy < CellSize.y && isOk; iy++)
              {
                var component = levelComponent[CellSize.x * x + ix, CellSize.y * y + iy, 0];
                if (component)
                {
                  if (!availableTypes.Contains(component.CellType))
                  {
                    if (item.RequireCells != null && item.RequireCells.Any(p => p == new Point(ix, iy)))
                    {
                      isOk = false;
                    }
                  }
                }
              }
            }

            if (isOk)
            {
              available.Add(item);
            }
          }

          var container = new GameObject(string.Format("{0}:{1}", x, y)).AddComponent<LevelEnviromentItemContainerComponent>();
          container.Preset = _preset;
          container.transform.SetParent(levelComponent.EnviromentContainer, false);
          var position = levelComponent.CoordinateConverter.ToWorld(new PositionF(x * CellSize.x, y * CellSize.y, 0f));
          container.transform.localPosition = position;
          containers.Add(container);

          if (available.Count != 0)
          {
            var index = UnityEngine.Random.Range(0, available.Count);
            var prefab = available[index];
            container.CreateItem(prefab);
            container.SetItemPosition();
            container.AttachItem();
          }
        }
      }

      _items = containers.ToArray();
    }

    private void Awake()
    {

    }
  }
}
