using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Commands;
using Injection;

namespace Game.Messages.Commands
{
  public class StartCommand : ICommand
  {
    [Inject]
    private GameStateController _stateController;

    public void Execute()
    {
      _stateController.Current = GameState.Menu;
    }
  }
}
