using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Components.Controllers
{
  public class LevelGridControllerComponent : MonoBehaviour
  {
    private void Awake()
    {
      
    }

    public bool IsMovable(Position position)
    {
      return false;
    }

    public CellStatus GetCell(Position position)
    {
      return null;
    }

    public CellStatus GetCell(Position position, CellDirection direction)
    {

      return null;
    }
  }
}
