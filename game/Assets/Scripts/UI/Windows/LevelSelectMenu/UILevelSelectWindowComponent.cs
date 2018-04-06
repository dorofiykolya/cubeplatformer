using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Windows
{
  public class UILevelSelectWindowComponent : UIWindowComponent
  {
    private Lifetime.Definition _definition;
    private Signal<int> _onClick;

    [SerializeField]
    private RectTransform _content;

    [SerializeField]
    private UILevelSelectPlaceholderComponent _itemPrefab;

    [SerializeField]
    private Text _title;

    [SerializeField]
    private Button _closeButton;

    private void Awake()
    {

    }

    public void SetTitle(string title)
    {
      _title.text = title;
    }

    public void SetLevels(GameSubLevelData[] levels)
    {
      InitializeDefinition();

      var iterator = 0;
      foreach (var level in levels)
      {
        var index = iterator;
        var placeholder = Instantiate(_itemPrefab, _content.transform, false).GetComponent<UILevelSelectPlaceholderComponent>();
        placeholder.SetTitle(iterator.ToString());
        placeholder.SetCategory(level.Category);
        placeholder.SubscribeOnClick(_definition.Lifetime, () =>
        {
          _onClick.Fire(index);
        });
        if (index == 0)
        {
          var selectable = placeholder.GetComponent<Selectable>();
          if (selectable)
          {
            selectable.Select();
          }
        }
        iterator++;
      }
    }

    public void SubscribeOnSelect(Lifetime lifetime, Action<int> listener)
    {
      InitializeDefinition();
      _onClick.Subscribe(lifetime, listener);
    }

    private void InitializeDefinition()
    {
      if (_definition == null)
      {
        _definition = Lifetime.Define(Lifetime.Eternal);
        _onClick = new Signal<int>(_definition.Lifetime);
      }
    }
  }
}
