using System;
using UnityEngine;

namespace Game.Views
{
  public class CellComponent : MonoBehaviour
  {
    [NonSerialized]
    public CellInfo CellInfo;

    private Transform _transform;

    [HideInInspector]
    public Position Position;
    [HideInInspector]
    public CellType CellType;

    public GameObject Content;
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
    }
  }
}