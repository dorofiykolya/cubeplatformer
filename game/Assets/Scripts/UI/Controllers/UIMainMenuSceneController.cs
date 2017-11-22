using Game.Inputs;
using Game.Managers;
using Game.Managers.Commands;
using Game.Messages;
using Game.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace Game.UI.Controllers
{
  public class UIMainMenuSceneController : UIController
  {
    protected override void OnInitialize()
    {
      Context.CommandMap.Map<StartMessage>().RegisterCommand(lifetime => new StartMainMenuCommand(Context, Lifetime), true);
    }

    private class StartMainMenuCommand : ICommand
    {
      private GameContext _context;
      private Lifetime _lifetime;

      public StartMainMenuCommand(GameContext context, Lifetime lifetime)
      {
        _context = context;
        _lifetime = lifetime;
      }

      public void Execute()
      {
        var lifeDefinition = Lifetime.Define(_lifetime);
        var sceneManager = _context.Managers.Get<GameSceneManager>();
        sceneManager.LoadScene(GameScenes.MainMenu, UnityEngine.SceneManagement.LoadSceneMode.Additive, (scene) =>
        {
          var gameObject = scene.GetRootGameObjects().FirstOrDefault(t => t.GetComponent<UIMainMenuComponent>() != null);
          var component = gameObject.GetComponent<UIMainMenuComponent>();
          var navigation = component.Navigation;
          var input = new MainMenuInputContext(_context, _context.Lifetime, _context.InputContext.Current);

          input.Subscribe(lifeDefinition.Lifetime, GameInput.Horizontal, InputPhase.Begin, e =>
          {
            if (e.Value > 0) navigation.GoToRight();
            else navigation.GoToLeft();
          });
          input.Subscribe(lifeDefinition.Lifetime, GameInput.Vertical, InputPhase.Begin, e =>
          {
            if (e.Value > 0) navigation.GoToTop();
            else navigation.GoToBottom();
          });
          input.Subscribe(lifeDefinition.Lifetime, GameInput.Cancel, InputPhase.Begin, e =>
          {
            navigation.GoToBack();
          });
        });
      }
    }

    private class MainMenuInputContext : InputContext
    {
      public MainMenuInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
      {
      }
    }
  }
}
