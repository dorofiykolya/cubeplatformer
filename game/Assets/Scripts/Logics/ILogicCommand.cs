using UnityEngine;
using System.Collections;

namespace Game.Logics
{
  public interface ILogicCommand
  {
    void Execute(ILogicEngine engine, ILogicAction action);
  }
}