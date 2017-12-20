using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Views.Components
{
  [RequireComponent(typeof(CellComponent)), ExecuteInEditMode]
  public class CellGizmoComponent : MonoBehaviour
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

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
    {
      var gizmos = LevelGizmos;
      if (gizmos != null)
      {
        var size = LevelGizmos.UnitSize;
        if (gizmos.ShowGizmosWireGrid)
        {
          Gizmos.color = new Color(0, 1, 0, 0.1f);
          Gizmos.DrawWireCube(transform.position + size / 2f, size);
        }
        if (gizmos.ShowGizmosGrid)
        {
          Gizmos.color = new Color(0, 1, 0, 0f);
          Gizmos.DrawCube(transform.position + size / 2f, size);
        }
      }
    }

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmosSelected()
    {
      var gizmos = LevelGizmos;
      if (gizmos != null)
      {
        if (gizmos.ShowGizmosSelectedGrid)
        {
          var size = LevelGizmos.UnitSize;
          Gizmos.color = new Color(1, 0, 0, 0.2f);
          Gizmos.DrawCube(transform.position + size / 2f, size);
        }
      }
    }
  }
}