using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Views
{
  [RequireComponent(typeof(CellComponent)), ExecuteInEditMode]
  public class CellGizmoComponent : MonoBehaviour
  {
    [HideInInspector, NonSerialized]
    public bool ShowGizmosSelectedGrid;
    [HideInInspector, NonSerialized]
    public bool ShowGizmosWireGrid;
    [HideInInspector, NonSerialized]
    public bool ShowGizmosGrid;

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
    {
      if (ShowGizmosWireGrid)
      {
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawWireCube(transform.position + transform.lossyScale / 2f, transform.lossyScale);
      }
      if (ShowGizmosGrid)
      {
        Gizmos.color = new Color(0, 1, 0, 0f);
        Gizmos.DrawCube(transform.position + transform.lossyScale / 2f, transform.lossyScale);
      }
    }

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmosSelected()
    {
      if (ShowGizmosSelectedGrid)
      {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(transform.position + transform.lossyScale / 2f, transform.lossyScale);
      }
    }
  }
}