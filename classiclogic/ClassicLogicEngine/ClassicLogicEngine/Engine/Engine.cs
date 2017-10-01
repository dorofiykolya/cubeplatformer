using System;
using ClassicLogic.Utils;

namespace ClassicLogic.Engine
{
  public class Engine
  {
    private readonly EngineState _state;

    public Engine(AIVersion aiVersion, LevelReader level)
    {
      var config = Constants.CONFIGURATION[aiVersion];


      _state = new EngineState(config, LevelParser.Parse(level, config.maxGuard));
    }

    public void FastForward(int tick)
    {

    }
  }
}
