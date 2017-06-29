using System.Diagnostics;
using UnityEngine;

namespace Game.Views
{
  [RequireComponent(typeof(LevelComponent)), ExecuteInEditMode]
  public class LevelGizmoComponent : MonoBehaviour
  {
    private bool _ShowGizmosSelectedGrid;
    private bool _ShowGizmosWireGrid;
    private bool _ShowGizmosGrid;

    public bool ShowGizmosSelectedGrid = true;
    public bool ShowGizmosWireGrid = true;
    public bool ShowGizmosGrid = true;

    [Conditional("UNITY_EDITOR")]
    private void Update()
    {
      if (ShowGizmosGrid != _ShowGizmosGrid)
      {
        foreach (var cell in GetComponent<LevelComponent>().Grid)
        {
          cell.GetComponent<CellGizmoComponent>().ShowGizmosGrid = ShowGizmosGrid;
        }

        _ShowGizmosGrid = ShowGizmosGrid;
      }

      if (ShowGizmosWireGrid != _ShowGizmosWireGrid)
      {
        foreach (var cell in GetComponent<LevelComponent>().Grid)
        {
          cell.GetComponent<CellGizmoComponent>().ShowGizmosWireGrid = ShowGizmosWireGrid;
        }

        _ShowGizmosWireGrid = ShowGizmosWireGrid;
      }

      if (ShowGizmosSelectedGrid != _ShowGizmosSelectedGrid)
      {
        foreach (var cell in GetComponent<LevelComponent>().Grid)
        {
          cell.GetComponent<CellGizmoComponent>().ShowGizmosSelectedGrid = ShowGizmosSelectedGrid;
        }

        _ShowGizmosSelectedGrid = ShowGizmosSelectedGrid;
      }
    }
  }
}