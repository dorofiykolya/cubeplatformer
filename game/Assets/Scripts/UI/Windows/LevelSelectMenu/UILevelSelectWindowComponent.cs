using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Windows
{
  public class UILevelSelectWindowComponent : UIWindowComponent
  {
    [SerializeField]
    private RectTransform _content;

    [SerializeField]
    private GameObject _itemPrefab;

    [SerializeField]
    private Text _title;

    [SerializeField]
    private Button _closeButton;

    private void Awake()
    {
      for (int i = 0; i < 30; i++)
      {
        Instantiate(_itemPrefab, _content.transform, false);
      }
      _closeButton.Select();
    }

    public void SetTitle(string title)
    {
      _title.text = title;
    }
  }
}
