using System;
using Game.Managers;
using Utils;

namespace Game
{
  public class GameStateManager : GameManager
  {
    private Signal<GameState, GameState> _onEnter;
    private Signal<GameState, GameState> _onExit;
    private Signal<GameState> _onChanged;
    private GameState _prev;
    private GameState _current;

    protected override void Preinitialize()
    {
      _current = _prev = GameState.Preloader;

      _onEnter = new Signal<GameState, GameState>(Lifetime);
      _onExit = new Signal<GameState, GameState>(Lifetime);
      _onChanged = new Signal<GameState>(Lifetime);
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
          _prev = _current != null ? _current : value;
          _onExit.Fire(_prev, value);
          _onEnter.Fire(_prev, value);
          _current = value;
          _onChanged.Fire(_current);
        }
      }
    }

    public void SubscribeOnExit(Lifetime lifetime, GameState exitState, Action<GameState> listenerNextState)
    {
      _onExit.Subscribe(lifetime, (exit, enter) =>
      {
        if (exit.Is(exitState))
        {
          listenerNextState(enter);
        }
      });
    }

    public void SubscribeOnEnter(Lifetime lifetime, GameState enterState, Action<GameState> listenerExitState)
    {
      _onEnter.Subscribe(lifetime, (exit, enter) =>
      {
        if (enter.Is(enterState))
        {
          listenerExitState(exit);
        }
      });
    }

    public void SubscribeOnChanged(Lifetime lifetime, Action<GameState> listenerCurrentState)
    {
      _onChanged.Subscribe(lifetime, listenerCurrentState);
    }
  }
}