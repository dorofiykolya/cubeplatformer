using System;
using Game.Controllers;
using Game.Inputs;
using Injection;
using Utils;

namespace Game.UI.Windows
{
  public class UILevelSelectWindow : UIWindow<UILevelSelectWindowComponent, UILevelSelectWindowData>
  {
    [Inject]
    private GameContext _gameContext;
    [Inject]
    private GameLevelController _levelController;
    [Inject]
    private UserLevelsContorller _userLevelsContorller;

    private UIInputContext _input;

    protected override void OnInitialize()
    {

    }

    protected override void OnOpen()
    {
      _input = new UIInputContext(_gameContext, Lifetime, _gameContext.InputContext.Current);
      _input.Subscribe(Lifetime, GameInput.Cancel, InputPhase.End, InputUpdate.Update, evt => Close());
      _input.Subscribe(Lifetime, GameInput.Action, InputPhase.End, InputUpdate.Update, evt =>
      {
        if (Component.Selected != null)
        {
          SelectHandler(Component.Selected.SubLevel);
        }
      });

      var levelData = _levelController.Levels.GetLevel(Data.Level);

      Component.SubscribeOnClose(Lifetime, Close);

      Component.SetTitle(levelData.Name);
      Component.SetLevels(GetDatas());
      Component.SubscribeOnSelect(Lifetime, SelectHandler);
    }

    private UILevelSelectLevelData[] GetDatas()
    {
      var list = ListPool<UILevelSelectLevelData>.Pop();

      var levelData = _levelController.Levels.GetLevel(Data.Level);
      var index = 0;
      foreach (var level in levelData.Levels)
      {
        var userData = _userLevelsContorller.GetData(Data.Level, index);
        list.Add(new UILevelSelectLevelData
        {
          Level = Data.Level,
          SubLevel = index,
          Stars = userData != null ? userData.Stars : 0,
          PlayCount = userData != null ? userData.PlayCount : 0,
          Name = levelData.Name,
          Description = levelData.Description,
          Category = level.Category,
          Available = level.Available || _userLevelsContorller.GetData(Data.Level, index - 1) != null
        });
        index++;
      }

      var result = list.ToArray();
      ListPool.Push(list);
      return result;
    }

    private void SelectHandler(int subLevel)
    {
      _levelController.LoadLevel(Data.Level, subLevel);
    }

    protected override void OnClose()
    {

    }

    private class UIInputContext : InputContext
    {
      public UIInputContext(GameContext context, Lifetime lifetime, InputContext parent) : base(context, lifetime, parent)
      {
      }
    }
  }
}
