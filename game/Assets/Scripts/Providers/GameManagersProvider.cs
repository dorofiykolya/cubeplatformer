﻿using System.Collections.Generic;
using Game.Managers;

namespace Game.Providers
{
  public class GameManagersProvider
  {
    public IEnumerable<GameManager> Providers(GameContext context)
    {
      yield return new GamePreloaderManager();
    }
  }
}