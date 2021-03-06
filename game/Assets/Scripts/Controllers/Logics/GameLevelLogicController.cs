using Game.Commands;
using Game.Inputs;
using Game.Logics;
using Game.Logics.Actions;
using Game.Messages;
using Injection;
using UnityEngine;
using Utils;
using ClassicLogicEngine = Game.Logics.Classic.ClassicLogicEngine;

namespace Game.Controllers
{
  public class GameLevelLogicController : GameController
  {
    private class PlayCommand : ICommand
    {
      private readonly GameLevelLogicController _logic;

      public PlayCommand(GameLevelLogicController logic)
      {
        _logic = logic;
      }

      public void Execute()
      {
        _logic._isPlay = true;
      }
    }

    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private GameStateController _stateController;
    [Inject]
    private UserLevelsContorller _userLevelsContorller;

    private Lifetime.Definition _levelDefinition;
    private bool _isPlay;

    protected override void OnInitialize()
    {
      _isPlay = false;
      _levelController.SubscribeOnLoaded(Lifetime, LevelLoadedHandler);
      _levelController.SubscribeOnUnloaded(Lifetime, LevelUnloadedHandler);

      Context.CommandMap.Map<PlayMessage>().RegisterCommand(Lifetime, lt => new PlayCommand(this));
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
      var guesture = new TouchGuestureContext(input, _levelDefinition.Lifetime);

      input.Subscribe(_levelDefinition.Lifetime, GameInput.Left, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.Left
        });
      });
      guesture.SubscribeSwipe(_levelDefinition.Lifetime, SwipeDirection.Left, () =>
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
      guesture.SubscribeSwipe(_levelDefinition.Lifetime, SwipeDirection.Right, () =>
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
      guesture.SubscribeSwipe(_levelDefinition.Lifetime, SwipeDirection.Up, () =>
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
      guesture.SubscribeSwipe(_levelDefinition.Lifetime, SwipeDirection.Down, () =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.Down
        });
      });

      input.Subscribe(_levelDefinition.Lifetime, GameInput.DigLeft, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.DigLeft
        });
      });
      guesture.SubscribeClick(_levelDefinition.Lifetime, new Rect(0f, 0f, 0.49f, 1f), () =>
         {
           logic.AddAction(new LogicActionInputAction
           {
             InputAction = InputAction.DigLeft
           });
         });

      input.Subscribe(_levelDefinition.Lifetime, GameInput.DigRight, InputPhase.Begin, InputUpdate.Update, evt =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.DigRight
        });
      });
      guesture.SubscribeClick(_levelDefinition.Lifetime, new Rect(0.51f, 0, 1f, 1f), () =>
      {
        logic.AddAction(new LogicActionInputAction
        {
          InputAction = InputAction.DigRight
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
        if (_isPlay)
        {
          passedTime += Context.Time.DeltaTime;
          while ((int)(passedTime / (1f / ticksPerSec)) > 0)
          {
            logic.FastForward(logic.Tick + 1);
            passedTime -= 1f / ticksPerSec;
            if (logic.IsFinished)
            {
              SaveLevel(levelInfo, logic);
              _levelController.Unload();
            }
          }
        }
      });
    }

    private void SaveLevel(CurrentLevelInfo levelInfo, ClassicLogicEngine logic)
    {
      var data = _userLevelsContorller.GetData(levelInfo.Level, levelInfo.SubLevel);
      if (data == null)
      {
        data = new UserLevelData
        {
          Level = levelInfo.Level,
          SubLevel = levelInfo.SubLevel,
          Time = logic.Tick / logic.TicksPerSeconds,
          PlayCount = 1,
          Stars = logic.Stars
        };
      }
      else
      {
        data.PlayCount++;
        data.Stars = Mathf.Max(data.Stars, logic.Stars);
      }
      _userLevelsContorller.SaveLevel(data);
    }

  }
}