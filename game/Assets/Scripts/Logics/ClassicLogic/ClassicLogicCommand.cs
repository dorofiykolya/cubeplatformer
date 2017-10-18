using System;
using ClassicLogic.Outputs;

namespace Game.Logics.ClassicLogic
{
  public abstract class ClassicLogicCommand<T> : ClassicLogicCommand where T : OutputEvent
  {
    protected abstract void Execute(T evt, ClassicLogicEngine engine);

    public override void Execute(OutputEvent evt, ClassicLogicEngine engine)
    {
      if (evt.GetType() == typeof(OutputEvent)) throw new InvalidOperationException();
      Execute((T)evt, engine);
    }
  }

  public abstract class ClassicLogicCommand
  {
    public abstract void Execute(OutputEvent evt, ClassicLogicEngine engine);
  }
}
