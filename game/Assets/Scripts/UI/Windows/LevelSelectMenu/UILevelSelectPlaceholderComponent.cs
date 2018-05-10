using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Windows
{
  public class UILevelSelectPlaceholderComponent : MonoBehaviour
  {
    private Lifetime.Definition _definition;
    private Signal _onClick;

    [SerializeField]
    private Text _title;
    [SerializeField]
    private Text _category;
    [SerializeField]
    private Text _playCount;
    [SerializeField]
    private Text _starts;

    public void SetTitle(string title)
    {
      _title.text = title;
    }

    public void SetCategory(LevelCategory category)
    {
      _category.text = category.ToString();
    }

    public void SubscribeOnClick(Lifetime lifetime, Action listener)
    {
      if (_definition == null)
      {
        _definition = Lifetime.Define(Lifetime.Eternal);
        _onClick = new Signal(_definition.Lifetime);
      }
      _onClick.Subscribe(lifetime, listener);
    }

    [UILink]
    public void FireClick()
    {
      if (_onClick != null)
      {
        _onClick.Fire();
      }
    }

    private void OnDestroy()
    {
      if (_definition != null)
      {
        var def = _definition;
        _definition = null;
        _onClick = null;
        def.Terminate();
      }
    }

    public void SetCount(int playCount)
    {
      _playCount.text = playCount.ToString();
    }

    public void SetStars(int stars)
    {
      _starts.text = stars.ToString();
    }
  }
}
