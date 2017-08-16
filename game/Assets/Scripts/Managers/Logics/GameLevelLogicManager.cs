using Game.Inputs;
using Game.Logics;
using Injection;
using Utils;

namespace Game.Managers
{
  public class GameLevelLogicManager : GameManager
  {
    [Inject]
    private GameLevelManager _levelManager;
    [Inject]
    private GameStateManager _stateManager;

    private Lifetime.Definition _levelDefinition;

    protected override void Initialize()
    {
      _levelManager.SubscribeOnLoaded(Lifetime, LevelLoadedHandler);
      _levelManager.SubscribeOnUnloaded(Lifetime, LevelUnloadedHandler);
    }

    private void LevelUnloadedHandler(GameClassicLevelInfo info)
    {
      if (_levelDefinition != null)
      {
        _levelDefinition.Terminate();
      }
    }

    private void LevelLoadedHandler(GameClassicLevelInfo info)
    {
      _levelDefinition = Lifetime.Definition.Define(Lifetime);
      _levelDefinition.Lifetime.AddAction(() =>
      {
        _stateManager.Current = _stateManager.Prev;
      });
      _stateManager.Current = GameState.ClassicPlayMode;
      var logic = info.Level.Logic.Engine(Context);

      var input = new LevelInputContenxt(Context, _levelDefinition.Lifetime, Context.InputContext);
      logic.AddAction(new LogicActionInitializePlayer(input.Controllers, logic.Tick + 1));
      input.SubscribeOnAddController(_levelDefinition.Lifetime, controller =>
      {
        logic.AddAction(new LogicActionAddPlayer(controller.Id, logic.Tick + 1));
      });
      input.SubscribeOnRemoveController(_levelDefinition.Lifetime, controller =>
      {
        logic.AddAction(new LogicActionRemovePlayer(controller.Id, logic.Tick + 1));
      });
      
      Context.Time.SubscribeOnUpdate(_levelDefinition.Lifetime, () =>
      {
        var ticks = 1;
        logic.FastForward(logic.Tick + ticks);
      });
    }
  }
}