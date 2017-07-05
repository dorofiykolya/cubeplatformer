using System;
using UnityEngine;

namespace Game.Views.Components
{
  public class CellComponent : MonoBehaviour
  {
    public CellInfo CellInfo;

    private Transform _transform;

    [HideInInspector]
    public Position Position;
    [HideInInspector]
    public CellType CellType;

    public CellContentComponent Content;
    public LevelComponent Level;

    public new Transform transform { get { return _transform ?? (_transform = base.transform); } }

    public void UpdatePosition()
    {
      transform.localPosition = Level.CoordinateConverter.ToWorld(Position);
    }

    public void SetContent(CellInfo cellInfo)
    {
      CellType = cellInfo.Type;
      CellInfo = cellInfo;
      UpdateContent();
    }

    public void UpdateContent()
    {
      if (Content != null)
      {
        DestroyImmediate(Content.gameObject);
      }
      if (CellInfo.Prefab != null)
      {
        Content = Instantiate(CellInfo.Prefab.gameObject).GetComponent<CellContentComponent>();
        Content.transform.SetParent(transform, false);
      }

    }
  }
}