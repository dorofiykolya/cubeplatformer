using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Prototypes.Elements
{
  [Serializable]
  public class ElementProperty : ScriptableObject
  {
    public ElementType Type;
    public int Power = 1;
  }
}
