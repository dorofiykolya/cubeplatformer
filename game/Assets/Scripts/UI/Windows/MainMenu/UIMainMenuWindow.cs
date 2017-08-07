using UnityEngine;
using System.Collections;
using Game.Managers;
using Game.UI.Components;
using Injection;

namespace Game.UI.Windows
{
  public class UIMainMenuWindow : UIWindow<UIMainManuComponent>
  {
    [Inject]
    private GameLevelManager _levelManager;

    protected override void Initialize()
    {
      UIMainManuComponent menuComponent = Component;
      if (menuComponent)
      {
        menuComponent.OnClassicClick.Subscribe(Lifetime, () =>
        {
          _levelManager.ResumeClassic();
          Close();
        });
        menuComponent.OnInfinityClick.Subscribe(Lifetime, _levelManager.ResumeInfinity);
      }
    }

    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {

    }
  }
}