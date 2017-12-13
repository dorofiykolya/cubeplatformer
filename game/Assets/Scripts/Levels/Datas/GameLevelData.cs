using System;
using Game.Views.Components;
using References;
using UnityEngine;

namespace Game
{
  [Serializable]
  public class GameLevelData
  {
    public string Name;
    public string Description;
    public GameLevelDataType DataType;
    [ResourceReferenceType(typeof(TextAsset))]
    public ResourceReferenceTextAsset LevelStringData;
    [ResourceReferenceType(typeof(CellPreset))]
    public ResourceReferenceCellPreset Preset;
    public GameSceneData Scene;

    [Serializable]
    public class ResourceReferenceEnvironmentComponent : ResourceReference<EnvironmentComponent> { }
    [Serializable]
    public class ResourceReferenceLevelComponent : ResourceReference<LevelComponent> { }
    [Serializable]
    public class ResourceReferenceTextAsset : ResourceReference<TextAsset> { }
    [Serializable]
    public class ResourceReferenceCellPreset : ResourceReference<CellPreset> { }
  }
}