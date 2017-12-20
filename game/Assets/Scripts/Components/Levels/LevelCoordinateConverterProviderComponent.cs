using UnityEngine;

namespace Game.Views
{
  public class LevelCoordinateConverterProviderComponent : MonoBehaviour
  {
    [SerializeField]
    private LevelCoordinateConverter _coordinateConverter;

    public ILevelCoordinateConverter Converter
    {
      get { return _coordinateConverter ?? (_coordinateConverter = ScriptableObject.CreateInstance<UnitLevelCoordinateConverter>()); }
    }
  }
}