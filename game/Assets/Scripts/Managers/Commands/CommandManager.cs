using Game.Modules;
using System;
using System.Collections.Generic;
using Utils;

namespace Game.Managers.Commands
{
  public class CommandMap
  {
    private Dictionary<Type, Container> _map;
    private readonly IContext _context;

    public CommandMap(IContext context)
    {
      _map = new Dictionary<Type, Container>();
      _context = context;
    }

    public ICommandMapper Map<TMessage>()
    {
      Container container;
      if (!_map.TryGetValue(typeof(TMessage), out container))
      {
        var lifetime = Lifetime.Define(_context.Lifetime);
        var mapper = new CommandMapper(lifetime.Lifetime, typeof(TMessage), _context);

        _map[typeof(TMessage)] = container = new Container
        {
          Lifetime = lifetime,
          Mapper = mapper
        };

        lifetime.Lifetime.AddAction(() =>
        {
          _map.Remove(typeof(TMessage));
        });
      }
      return container.Mapper;
    }

    public void Tell(object message)
    {
      Container container;
      if (_map.TryGetValue(message.GetType(), out container))
      {
        container.Mapper.Tell(message);
      }
    }

    private class Container
    {
      public Lifetime.Definition Lifetime;
      public CommandMapper Mapper;
    }
  }
}
