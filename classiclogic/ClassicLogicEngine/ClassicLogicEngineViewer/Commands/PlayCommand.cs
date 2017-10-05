using System;
using ClassicLogic.Outputs;

namespace ClassicLogicEngineViewer.Commands
{
  public abstract class PlayCommand<T> : PlayCommand where T : OutputEvent
  {
    protected abstract void Execute(T value, TileContainer tileContainer);

    public override void Execute(OutputEvent evt, TileContainer tileContainer)
    {
      if (evt.GetType() == typeof(OutputEvent)) throw new ArgumentException();
      Execute((T)evt, tileContainer);
    }
  }

  public abstract class PlayCommand
  {
    public abstract void Execute(OutputEvent evt, TileContainer tileContainer);
  }
}
