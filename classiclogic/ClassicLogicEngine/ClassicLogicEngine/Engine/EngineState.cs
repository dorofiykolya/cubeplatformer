using System;
namespace ClassicLogic.Engine
{
  public class EngineState
  {
    private readonly LevelMap _levelMap;
    private readonly Configuration _config;

    public EngineState(Configuration config, LevelMap levelMap)
    {
      _config = config;
      _levelMap = levelMap;
    }
  }
}
