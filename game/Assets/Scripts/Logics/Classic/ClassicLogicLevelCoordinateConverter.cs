﻿using System;
using UnityEngine;

namespace Game.Logics.Classic
{
  [Serializable]
  public class ClassicLogicLevelCoordinateConverter : LevelCoordinateConverter
  {
    public LevelSize Size;

    public override Vector3 ToWorld(PositionF logicPosition)
    {
      return new Vector3(logicPosition.X, Size.Y - 1 - logicPosition.Y, logicPosition.Z);
    }

    public override PositionF ToPositionF(Vector3 worldPosition)
    {
      return new PositionF(worldPosition.x, Size.Y - 1 - worldPosition.y, worldPosition.z);
    }

    public override Position ToPosition(Vector3 worldPosition)
    {
      return new Position((int)worldPosition.x, Size.Y - 1 - (int)worldPosition.y, (int)worldPosition.z);
    }
  }
}
