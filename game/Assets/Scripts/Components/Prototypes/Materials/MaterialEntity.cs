using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Prototypes;
using Game.Prototypes.Elements;

namespace Game.Components.Prototypes
{
  [Serializable]
  public class MaterialEntity
  {
    public int StateValue;
    public MaterialState State;
    public MaterialProperty Property;
    public bool IsBurnt;

    public void Update()
    {
      if (State == MaterialState.Burn)
      {
        StateValue++;
        if (StateValue >= MaterialStateRules.MaxStateValue)
        {
          State = MaterialState.Burnt;
        }
      }
    }
  }
}
