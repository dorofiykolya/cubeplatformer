﻿namespace Game
{
  public class GameState : State
  {
    public static readonly GameState Preloader = new GameState("Preloader", 0);
    public static readonly GameState Menu = new GameState("Menu", 1);

    public GameState(string name, int value, State parent = null) : base(name, value, parent)
    {
    }
  }
}