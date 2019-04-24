using System;
using Utils;

namespace Game.Commands
{
  public interface ICommandMapper
  {
    Lifetime.Definition RegisterCommand(Lifetime lifetime, Func<Lifetime, ICommand> factory, bool oneTime = false);
  }
}
