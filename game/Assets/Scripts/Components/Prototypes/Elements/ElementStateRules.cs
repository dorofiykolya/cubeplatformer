using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Prototypes.Elements
{
  public class ElementStateRules
  {
    public static void Apply(ElementEntity from, ElementEntity to)
    {
      switch (from.Property.Type)
      {
        case ElementType.Fire:
          ApplyFire(from, to);
          break;
        case ElementType.Water:
          ApplyWater(from, to);
          break;
      }
    }

    private static void ApplyFire(ElementEntity from, ElementEntity to)
    {
      switch (to.Property.Type)
      {
        case ElementType.Water:
          to.Lifes -= from.Property.Power;
          break;
      }
    }

    private static void ApplyWater(ElementEntity from, ElementEntity to)
    {
      switch (to.Property.Type)
      {
        case ElementType.Fire:
          to.Lifes -= from.Property.Power;
          break;
      }
    }
  }
}
