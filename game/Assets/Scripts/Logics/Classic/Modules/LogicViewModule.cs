using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Views.Components;

namespace Game.Logics.Classics
{
  public class LogicViewModule : ILogicModule
  {
    private LogicModules _logicModules;
    private ILevelCoordinateConverter _levelCoordinateConverter;
    private GameContext _context;
    private LevelComponent _level;
    private Dictionary<int, CellPlayerContentComponent> _players;
    private Dictionary<int, CellGuardContentComponent> _guards;
    private LevelSize _size;

    public LogicViewModule(LogicModules logicModules)
    {
      _logicModules = logicModules;
      _guards = new Dictionary<int, CellGuardContentComponent>();
      _players = new Dictionary<int, CellPlayerContentComponent>();
    }

    public void Initialize(GameContext gameContext, ILevelCoordinateConverter levelCoordinateConverter, LevelComponent level, LevelSize size)
    {
      _size = size;
      _context = gameContext;
      _level = level;
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
      _guards[id].gameObject.SetActive(true);
      _guards[id].transform.localPosition = _levelCoordinateConverter.ToWorld(new PositionF(bornX, _size.Y - bornY, 0));
    }

    public void GuardStop(int curGuard)
    {

    }

    public void GuardPosition(int curGuardId, float x, float y)
    {
      _guards[curGuardId].transform.localPosition = _levelCoordinateConverter.ToWorld(new PositionF(x, _size.Y - y, 0));
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
      _level[x, y, 0].gameObject.SetActive(false);
    }

    public void DisplayBlock(Position position)
    {
      _level[position.X, position.Y, position.Z].gameObject.SetActive(true);
    }

    public void HidePlayer(int playerId)
    {
      _players[playerId].gameObject.SetActive(false);
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

    public void PlayerPosition(int playerId, float x, float y)
    {
      _players[playerId].transform.localPosition = _levelCoordinateConverter.ToWorld(new PositionF(x, _size.Y - y, 0));
    }

    public void AddGuard(int id, CellComponent cell)
    {
      _guards[id] = cell.Content as CellGuardContentComponent;
      cell.Content.transform.SetParent(_level.transform);
    }

    public void PlayerAdd(LogicPlayer player)
    {
      _players[player.Id] = _context.Managers.Get<GamePlayersManager>().CreatePlayer(player.Id, _level.transform);
    }
  }
}