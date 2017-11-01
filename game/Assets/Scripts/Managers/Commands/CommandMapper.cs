using Game.Modules;
using System;
using System.Collections.Generic;
using Utils;

namespace Game.Managers.Commands
{
  public class CommandMapper : ICommandMapper
  {
    private Lifetime _lifetime;
    private Type _messageType;
    private readonly IContext _context;
    private List<Type> _map;
    private List<CommandFactory> _commands;

    public CommandMapper(Lifetime lifetime, Type messageType, IContext context)
    {
      _lifetime = lifetime;
      _messageType = messageType;
      _context = context;
      _commands = new List<CommandFactory>();
    }

    public Lifetime RegisterCommand(Func<Lifetime, ICommand> factory, bool oneTime = false)
    {
      var lifetime = Lifetime.Define(_lifetime);
      var commandFactory = new CommandFactory(factory, oneTime, lifetime);
      _commands.Add(commandFactory);
      lifetime.Lifetime.AddAction(() =>
      {
        _commands.Remove(commandFactory);
      });
      return lifetime.Lifetime;
    }

    public void Tell(object message)
    {
      foreach (var factory in _commands.ToArray())
      {
        var command = factory.Factory(factory.Lifetime.Lifetime);
        _context.Injector.Map(_messageType).ToValue(message);
        _context.Injector.Inject(command);
        command.Execute();
        _context.Injector.Unmap(_messageType);
        if (factory.OneTime)
        {
          factory.Lifetime.Terminate();
        }
      }
    }

    private class CommandFactory
    {
      public Func<Lifetime, ICommand> Factory;
      public bool OneTime;
      public Lifetime.Definition Lifetime;

      public CommandFactory(Func<Lifetime, ICommand> factory, bool oneTime, Lifetime.Definition lifetime)
      {
        Factory = factory;
        OneTime = oneTime;
        Lifetime = lifetime;
      }
    }
  }
}
