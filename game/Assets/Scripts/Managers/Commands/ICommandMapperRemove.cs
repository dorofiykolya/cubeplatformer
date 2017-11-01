using System;

namespace Game.Managers.Commands
{
  public interface ICommandMapperRemove
  {
    void Remove<T>() where T : ICommand;
    void Remove(Type type);
  }
}
