using System;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class GameSubLevelData
  {
    public LevelCategory Category;
    public GameLevelDataType DataType;
    public TextAsset LevelStringData;
    public CellPreset Preset;
    public GameSceneData Scene;
    public bool Available;
  }
}
