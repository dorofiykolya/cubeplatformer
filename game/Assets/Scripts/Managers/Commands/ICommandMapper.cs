using System;
using Utils;

namespace Game.Managers.Commands
{
  public interface ICommandMapper
  {
    Lifetime RegisterCommand(Func<Lifetime, ICommand> factory, bool oneTime = false);
  }
}
