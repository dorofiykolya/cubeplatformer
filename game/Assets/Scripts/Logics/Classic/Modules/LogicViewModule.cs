using System;
using Game.Views.Components;

namespace Game.Logics.Classics
{
  public class LogicViewModule : ILogicModule
  {
    private LogicModules _logicModules;
    private ILevelCoordinateConverter _levelCoordinateConverter;

    public LogicViewModule(LogicModules logicModules)
    {
      _logicModules = logicModules;
    }

    public void Initialize(ILevelCoordinateConverter levelCoordinateConverter)
    {
      _levelCoordinateConverter = levelCoordinateConverter;
    }

    public void PostTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void PreTick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void Tick(ILogicEngine logicEngine, int currentTick, int deltaTick)
    {

    }

    public void GuardReborn(int id, int bornX, int bornY, CellType reborn)
    {

    }

    public void GuardStop(int curGuard)
    {

    }

    public void GuardPosition(int curGuardId, float x, float y)
    {

    }

    public void GuardShape(LogicActorShape newShape)
    {

    }

    public void GuardPlay(int curGuardId)
    {

    }

    public void PlayerPlay(int playerId, LogicActorAction playerShape)
    {

    }

    public void HideBlock(int x, int y)
    {

    }

    public void DisplayBlock(Position position)
    {

    }

    public void HidePlayer(int playerId)
    {

    }

    public void PlayerStop(int playerId)
    {

    }

    public void HoleObjRemoveFromScene(LogicHole holeObj)
    {

    }

    public void HoleObjAddToScene(LogicHole holeObj)
    {

    }

    public void HoleObjPlay(int playerId, LogicActorAction playerShape)
    {

    }

    public void PlayerPosition(float x, float y)
    {
      
    }

    public void AddGuard(int id, CellComponent cell)
    {
      
    }
  }
}