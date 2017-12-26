using UnityEngine;

namespace Game
{
  public class LevelCoordinateConverterProviderComponent : MonoBehaviour
  {
    [SerializeField]
    private LevelCoordinateConverter _coordinateConverter;

    public void SetConverter(LevelCoordinateConverter converter)
    {
      _coordinateConverter = converter;
    }

    public ILevelCoordinateConverter Converter
    {
      get { return _coordinateConverter ?? (_coordinateConverter = ScriptableObject.CreateInstance<UnitLevelCoordinateConverter>()); }
    }
  }
}