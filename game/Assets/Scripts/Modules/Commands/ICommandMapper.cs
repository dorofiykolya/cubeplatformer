using System;
using Utils;

namespace Game.Commands
{
  public interface ICommandMapper
  {
    Lifetime RegisterCommand(Func<Lifetime, ICommand> factory, bool oneTime = false);
  }
}
