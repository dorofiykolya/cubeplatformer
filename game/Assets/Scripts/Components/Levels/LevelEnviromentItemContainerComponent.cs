using System.Diagnostics;
using UnityEngine;

namespace Game.Components.Levels
{
  public class LevelEnviromentItemContainerComponent : MonoBehaviour
  {
    private LevelGizmoComponent _levelGizmos;

    private LevelGizmoComponent LevelGizmos
    {
      get
      {
        if (_levelGizmos == null)
        {
          _levelGizmos = GetComponentInParent<LevelGizmoComponent>();
        }
        return _levelGizmos;
      }
    }

    public LevelEnvironmentItemComponent Prefab;
    public LevelEnvironmentPreset Preset;
    public LevelEnvironmentItemComponent Item;

    public void CreateItem(LevelEnvironmentItemComponent prefab)
    {
      Prefab = prefab;
      Item = Instantiate(prefab);
    }

    public void DestroyItemImmediate()
    {
      if (Item != null)
      {
        DestroyImmediate(Item.gameObject);
        Item = null;
        Prefab = null;
      }
    }

    public void SetItemPosition()
    {
      Item.transform.localPosition = new Vector3(3, 3);
    }

    public void AttachItem()
    {
      Item.transform.SetParent(transform, false);
    }

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
    {
      if (!LevelGizmos || !LevelGizmos.ShowGizmoEnviroment) return;

      var size = LevelGizmos.UnitSize;

      Gizmos.color = new Color(0, 1, 0, 0.2f);
      var cellSize = new Vector3(size.x * LevelEnvironmentComponent.CellSize.x,
        size.y * LevelEnvironmentComponent.CellSize.y, 1);

      Gizmos.DrawWireCube(transform.position + cellSize / 2f, cellSize);

      Gizmos.color = new Color(0, 1, 0, 0f);
      Gizmos.DrawCube(transform.position + cellSize / 2f, cellSize);
    }

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmosSelected()
    {
      if (!LevelGizmos || !LevelGizmos.ShowGizmoEnviroment) return;

      var size = LevelGizmos.UnitSize;

      Gizmos.color = new Color(0, 1, 0, 0.2f);
      var cellSize = new Vector3(size.x * LevelEnvironmentComponent.CellSize.x,
        size.y * LevelEnvironmentComponent.CellSize.y, 1);

      //Gizmos.DrawWireCube(transform.position + cellSize / 2f, cellSize);

      Gizmos.color = new Color(1, 0, 0, 0.2f);
      Gizmos.DrawCube(transform.position + cellSize / 2f, cellSize);
    }
  }
}
