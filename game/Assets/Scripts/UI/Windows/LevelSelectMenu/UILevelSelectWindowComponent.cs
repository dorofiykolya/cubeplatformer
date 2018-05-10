using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Windows
{
  public class UILevelSelectWindowComponent : UIWindowComponent
  {
    private Lifetime.Definition _definition;

    [SerializeField]
    private RectTransform _content;

    [SerializeField]
    private UILevelSelectPlaceholderComponent _itemPrefab;

    [SerializeField]
    private Text _title;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private UILevelSelectInfoPlaceholderComponent _infoPlaceholder;

    public void SetTitle(string title)
    {
      _title.text = title;
    }

    public UILevelSelectLevelData Selected
    {
      get { return _infoPlaceholder.Selected; }
    }

    public void SetLevels(UILevelSelectLevelData[] levels)
    {
      InitializeDefinition();

      foreach (var level in levels)
      {
        var placeholder = Instantiate(_itemPrefab, _content.transform, false).GetComponent<UILevelSelectPlaceholderComponent>();
        placeholder.SetTitle(level.Name + ' ' + level.SubLevel);
        placeholder.SetCount(level.PlayCount);
        placeholder.SetStars(level.Stars);
        placeholder.SetCategory(level.Category);
        placeholder.SubscribeOnClick(_definition.Lifetime, () =>
        {
          _infoPlaceholder.Select(level);
        });
        if (level.SubLevel == 0)
        {
          var selectable = placeholder.GetComponent<Selectable>();
          if (selectable)
          {
            selectable.Select();
          }
        }
      }
    }

    public void SubscribeOnSelect(Lifetime lifetime, Action<int> listener)
    {
      _infoPlaceholder.SubscribeOnClick(lifetime, listener);
    }

    private void InitializeDefinition()
    {
      if (_definition == null)
      {
        _definition = Lifetime.Define(Lifetime.Eternal);
      }
    }

    private void OnDestroy()
    {
      if (_definition != null)
      {
        var def = _definition;
        _definition = null;
        def.Terminate();
      }
    }
  }
}
