using System;

namespace Game.Prototypes.Elements
{
  [Serializable]
  public class ElementEntity
  {
    public ElementState State;
    public ElementProperty Property;
    public int Lifes = 10;
  }
}
