using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Windows
{
  public class UILevelSelectInfoPlaceholderComponent : MonoBehaviour
  {
    private Lifetime.Definition _definition;
    private Signal<int> _onClick;

    [SerializeField]
    private Text _title;
    [SerializeField]
    private Text _category;
    [SerializeField]
    private Text _playCount;
    [SerializeField]
    private Text _starts;
    [SerializeField]
    private Text _description;
    [SerializeField]
    private Button _playButton;

    private UILevelSelectLevelData _level;

    public UILevelSelectLevelData Selected
    {
      get { return _level; }
    }

    public void SubscribeOnClick(Lifetime lifetime, Action<int> listener)
    {
      OnClick.Subscribe(lifetime, listener);
    }

    [UILink]
    public void FirePlay()
    {
      OnClick.Fire(_level.SubLevel);
    }

    public void Select(UILevelSelectLevelData level)
    {
      _level = level;

      _title.text = level.Name;
      _description.text = level.Description;
      _category.text = level.Category.ToString();
      _playCount.text = level.PlayCount.ToString();
      _starts.text = level.Stars.ToString();

      _playButton.interactable = level.Available;
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

    private Signal<int> OnClick
    {
      get { return _onClick ?? (_onClick = new Signal<int>(Lifetime)); }
    }


    private Lifetime Lifetime
    {
      get { return _definition != null ? _definition.Lifetime : (_definition = Lifetime.Define(Lifetime.Eternal)).Lifetime; }
    }
  }
}
