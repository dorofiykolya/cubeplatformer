using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Prototypes.Components;

namespace Game.Components
{
  public class ClimbMaterialComponent : MaterialComponent
  {
    public ClimbType ClimbType;

    public virtual bool Movable(IMovable movable)
    {
      return !Material.IsBurnt;
    }

    public virtual bool CanJump(IMovable movable)
    {
      return !Material.IsBurnt;
    }
  }
}
