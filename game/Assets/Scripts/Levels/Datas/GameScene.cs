using System;

namespace Game
{
  [Serializable]
  public struct GameScene
  {
    public static implicit operator string(GameScene scene)
    {
      return scene.Name;
    }

    public static implicit operator GameScene(string scene)
    {
      return new GameScene { Name = scene };
    }

    public string Name;
    public int Index;
  }
}