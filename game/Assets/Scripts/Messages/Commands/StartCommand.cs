using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Managers.Commands;
using Injection;

namespace Game.Messages.Commands
{
  public class StartCommand : ICommand
  {
    [Inject]
    private GameStateManager _stateManager;

    public void Execute()
    {
      _stateManager.Current = GameState.Menu;
    }
  }
}
