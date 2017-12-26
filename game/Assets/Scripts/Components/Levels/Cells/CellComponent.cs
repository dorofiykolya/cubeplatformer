using UnityEngine;

namespace Game.Components
{
  public class CellComponent : MonoBehaviour
  {
    public CellInfo CellInfo;

    [HideInInspector]
    public Position Position;
    [HideInInspector]
    public CellType CellType;
    public CellDirection CellDirection;
    public string CellId;

    public CellContentComponent Content;

    public LevelComponent Level
    {
      get { return GetComponentInParent<LevelComponent>(); }
    }

    public void UpdatePosition()
    {
      transform.localPosition = Level.CoordinateConverter.ToWorld(Position);
    }

    public void SetContent(CellInfo cellInfo)
    {
      CellType = cellInfo.Type;
      CellDirection = cellInfo.Direction;
      CellId = cellInfo.Id;
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
#if UNITY_EDITOR
        UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(Content.gameObject, CellInfo.Prefab.gameObject);
#endif
      }

      CellGuardComponentStrategy.Process(this, CellType);
      CellCoinComponentStrategy.Process(this, CellType);
      CellPlayerComponentStrategy.Process(this, CellType);
    }
  }
}