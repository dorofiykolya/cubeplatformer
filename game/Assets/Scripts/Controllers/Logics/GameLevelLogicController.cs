using Game.Inputs;
using Game.Logics;
using Game.Logics.Actions;
using Injection;
using Utils;
using ClassicLogicEngine = Game.Logics.Classic.ClassicLogicEngine;

namespace Game.Controllers
{
  public class GameLevelLogicController : GameController
  {
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private GameStateController _stateController;

    private Lifetime.Definition _levelDefinition;

    protected override void OnInitialize()
    {
      _levelController.SubscribeOnLoaded(Lifetime, LevelLoadedHandler);
      _levelController.SubscribeOnUnloaded(Lifetime, LevelUnloadedHandler);
    }

    private void LevelUnloadedHandler(CurrentLevelInfo info)
    {
      if (_levelDefinition != null)
      {
        _levelDefinition.Terminate();
      }
    }

    private void LevelLoadedHandler(CurrentLevelInfo levelInfo)
    {
      var info = levelInfo.ClassicLevel;
      if (info == null) return;

      _levelDefinition = Lifetime.Definition.Define(Lifetime);
      _levelDefinition.Lifetime.AddAction(() =>
      {
        _stateController.Current = _stateController.Prev;
      });
      _stateController.Current = GameState.ClassicPlayMode;
      var logic = new Game.Logics.Classic.ClassicLogicEngine(Context, _levelDefinition.Lifetime, info.Level);
      logic.ViewContext.Preset = info.Preset;

      var input = new LevelInputContenxt(Context, _levelDefinition.Lifetime, Context.InputContext);

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Left, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.Left
        });
      });

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Right, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.Right
        });
      });

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Up, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.Up
        });
      });

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Down, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.Down
        });
      });

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Action, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = evt.Value < 0 ? InputAction.DigLeft : InputAction.DigRight
        });
      });
      input.Subscribe(_levelDefinition.Lifetime, GameInput.Cancel, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        _levelController.Unload();
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