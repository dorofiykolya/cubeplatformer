using Game.UI.HUDs;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Windows
{
  public class UIHUDMainManuComponent : UIHUDComponent
  {
    [SerializeField]
    private Button[] Buttons;

    private int _current;

    private Lifetime.Definition _definition;

    public ISignalSubsribe OnSettingsClick { get; private set; }
    public ISignalSubsribe OnClassicClick { get; private set; }
    public ISignalSubsribe OnInfinityClick { get; private set; }
    public ISignalSubsribe OnExitClick { get; private set; }

    private void Awake()
    {
      _definition = Lifetime.Define(Lifetime.Eternal);

      OnSettingsClick = new Signal(_definition.Lifetime);
      OnClassicClick = new Signal(_definition.Lifetime);
      OnInfinityClick = new Signal(_definition.Lifetime);
      OnExitClick = new Signal(_definition.Lifetime);

      _current = 0;
      UpdateButton(true);
    }

    private void OnDestroy()
    {
      _definition.Terminate();
    }

    private void UpdateButton(bool selected)
    {
      Buttons[_current].GetComponent<Image>().color = selected ? Color.red : Color.white;
    }

    [UILink]
    public void FireSettingClick()
    {
      ((Signal)OnSettingsClick).Fire();
    }

    [UILink]
    public void FireClassicClick()
    {
      ((Signal)OnClassicClick).Fire();
    }

    [UILink]
    public void FireInfinityClick()
    {
      ((Signal)OnInfinityClick).Fire();
    }

    [UILink]
    public void FireExitClick()
    {
      ((Signal)OnExitClick).Fire();
    }

    public void Up()
    {
      var next = _current - 1;
      if (next < 0) next = Buttons.Length - 1;
      UpdateButton(false);
      _current = next;
      UpdateButton(true);
    }

    public void Down()
    {
      var next = _current + 1;
      if (next >= Buttons.Length) next = 0;
      UpdateButton(false);
      _current = next;
      UpdateButton(true);
    }

    public void Submit()
    {
      Buttons[_current].onClick.Invoke();
    }
  }
}