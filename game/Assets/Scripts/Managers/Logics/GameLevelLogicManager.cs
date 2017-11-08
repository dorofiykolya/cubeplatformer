using Game.Inputs;
using Game.Logics;
using Game.Logics.Actions;
using Injection;
using Utils;
using ClassicLogicEngine = Game.Logics.Classic.ClassicLogicEngine;

namespace Game.Managers
{
  public class GameLevelLogicManager : GameManager
  {
    [Inject]
    private GameLevelManager _levelManager;
    [Inject]
    private GameStateManager _stateManager;

    private Lifetime.Definition _levelDefinition;

    protected override void OnInitialize()
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
        _levelDefinition = null;
      });
      _stateManager.Current = GameState.ClassicPlayMode;
      var logic = new Game.Logics.Classic.ClassicLogicEngine(Context, _levelDefinition.Lifetime, info.Level);
      logic.ViewContext.Preset = info.Preset;

      var input = new LevelInputContenxt(Context, _levelDefinition.Lifetime, Context.InputContext);

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Horizontal, InputPhase.Begin, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = evt.Value > 0 ? InputAction.Right : InputAction.Left
        });
      });
      input.Subscribe(_levelDefinition.Lifetime, GameInput.Vertical, InputPhase.Begin, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = evt.Value > 0 ? InputAction.Up : InputAction.Down
        });
      });
      input.Subscribe(_levelDefinition.Lifetime, GameInput.Action, InputPhase.Begin, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = evt.Value < 0 ? InputAction.DigLeft : InputAction.DigRight
        });
      });
      input.Subscribe(_levelDefinition.Lifetime, GameInput.Cancel, InputPhase.Begin, evt =>
      {
        _levelManager.Unload();
      });

      logic.AddAction(new LogicActionInitializePlayer(input.Controllers, logic.Tick + 1));
      input.SubscribeOnAddController(_levelDefinition.Lifetime, controller =>
      {
        logic.AddAction(new LogicActionAddPlayer(controller.Id, logic.Tick + 1));
      });
      input.SubscribeOnRemoveController(_levelDefinition.Lifetime, controller =>
      {
        logic.AddAction(new LogicActionRemovePlayer(controller.Id, logic.Tick + 1));
      });

      var ticksPerSec = logic.TicksPerSeconds;
      var passedTime = 0f;

      Context.Time.SubscribeOnUpdate(_levelDefinition.Lifetime, () =>
      {
        passedTime += Context.Time.DeltaTime;
        while ((int)(passedTime / (1f / ticksPerSec)) > 0)
        {
          logic.FastForward(logic.Tick + 1);
          passedTime -= 1f / ticksPerSec;
        }
      });
    }
  }
}