using System;
using Utils;

namespace Game
{
  public class GameStateManager
  {
    private readonly Signal<GameState, GameState> _onEnter;
    private readonly Signal<GameState, GameState> _onExit;
    private readonly Signal<GameState> _onChanged;
    private readonly Lifetime _lifetime;
    private GameState _prev;
    private GameState _current;

    public GameStateManager(Lifetime lifetime)
    {
      _lifetime = lifetime;
      _onEnter = new Signal<GameState, GameState>(_lifetime);
      _onExit = new Signal<GameState, GameState>(_lifetime);
      _onChanged = new Signal<GameState>(_lifetime);
    }

    public GameState Prev
    {
      get { return _prev; }
    }

    public GameState Current
    {
      get { return _current; }
      set
      {
        if (value != _current)
        {
          _prev = _current;
          _current = value;
        }
      }
    }

    public void SubscribeOnExit(Lifetime lifetime, GameState exitState, Action<GameState> listenerNextState)
    {
      _onExit.Subscribe(lifetime, (exit, enter) =>
      {
        if (exit.Is(exitState)) listenerNextState(enter);
      });
    }

    public void SubscribeOnEnter(Lifetime lifetime, GameState enterState, Action<GameState> listenerExitState)
    {
      _onEnter.Subscribe(lifetime, (exit, enter) =>
      {
        if (enter.Is(enterState)) listenerExitState(exit);
      });
    }

    public void SubscribeOnChanged(Lifetime lifetime, Action<GameState> listenerCurrentState)
    {
      _onChanged.Subscribe(lifetime, listenerCurrentState);
    }
  }
}