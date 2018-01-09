using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Prototypes;
using Game.Prototypes.Elements;

namespace Game.Components.Prototypes
{
  public class MaterialStateRules
  {
    public const int MaxStateValue = 100;

    public static void Apply(MaterialEntity materialEntity, ElementEntity elementEntity)
    {
      switch (elementEntity.Property.Type)
      {
        case ElementType.Fire:
          ApplyFire(materialEntity, elementEntity);
          break;
        case ElementType.Freeze:
          ApplyFreeze(materialEntity, elementEntity);
          break;
        case ElementType.Water:
          ApplyWater(materialEntity, elementEntity);
          break;
        case ElementType.Wind:
          ApplyWind(materialEntity, elementEntity);
          break;
        case ElementType.Electricity:
          ApplyElectricity(materialEntity, elementEntity);
          break;
      }
    }

    private static void ApplyFire(MaterialEntity entity, ElementEntity element)
    {
      switch (entity.State)
      {
        case MaterialState.Dry:
        case MaterialState.Fried:
          entity.StateValue += element.Property.Power;
          if (entity.StateValue >= MaxStateValue)
          {
            entity.StateValue = 0;
            entity.State = MaterialState.Burn;
          }
          break;
        case MaterialState.Wet:
          entity.StateValue -= element.Property.Power;
          if (entity.StateValue <= 0)
          {
            entity.StateValue = 0;
            entity.State = MaterialState.Dry;
          }
          break;
        case MaterialState.Burn:
          entity.StateValue += element.Property.Power;
          if (entity.StateValue >= MaxStateValue)
          {
            entity.StateValue = 0;
            entity.State = MaterialState.Burnt;
          }
          break;
        case MaterialState.Freeze:
          entity.StateValue -= element.Property.Power;
          if (entity.StateValue <= 0)
          {
            entity.StateValue = MaxStateValue;
            entity.State = MaterialState.Wet;
          }
          break;
        case MaterialState.Electricity:
          break;
        case MaterialState.Burnt:
          break;
      }
    }

    private static void ApplyFreeze(MaterialEntity entity, ElementEntity element)
    {

    }

    private static void ApplyWater(MaterialEntity entity, ElementEntity element)
    {
      switch (entity.State)
      {
        case MaterialState.Dry:
        case MaterialState.Fried:
          entity.StateValue -= element.Property.Power;
          if (entity.StateValue <= 0)
          {
            entity.StateValue = 0;
            entity.State = MaterialState.Wet;
          }
          break;
        case MaterialState.Wet:
          entity.StateValue += element.Property.Power;
          if (entity.StateValue >= MaxStateValue)
          {
            entity.StateValue = MaxStateValue;
          }
          break;
        case MaterialState.Burn:
          entity.StateValue -= element.Property.Power;
          if (entity.StateValue <= 0)
          {
            entity.StateValue = MaxStateValue;
            entity.State = MaterialState.Dry;
          }
          break;
        case MaterialState.Freeze:
          break;
        case MaterialState.Electricity:
          break;
        case MaterialState.Burnt:
          break;
      }
    }

    private static void ApplyWind(MaterialEntity entity, ElementEntity element)
    {

    }

    private static void ApplyElectricity(MaterialEntity entity, ElementEntity element)
    {

    }
  }
}
