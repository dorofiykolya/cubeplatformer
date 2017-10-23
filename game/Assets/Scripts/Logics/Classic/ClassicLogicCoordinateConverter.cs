using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Logics.Classic
{
  public class ClassicLogicCoordinateConverter : ILevelCoordinateConverter
  {
    private readonly int _maxTileX;
    private readonly int _maxTileY;

    public ClassicLogicCoordinateConverter(int maxTileX, int maxTileY)
    {
      _maxTileX = maxTileX;
      _maxTileY = maxTileY;
    }

    public Vector3 ToWorld(PositionF logicPosition)
    {
      return new Vector3(logicPosition.X, _maxTileY - logicPosition.Y, 0);
    }

    public PositionF ToPosition(Vector3 worldPosition)
    {
      return new PositionF(worldPosition.x, _maxTileY - worldPosition.y, 0);
    }
  }
}
