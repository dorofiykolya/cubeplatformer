using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.Components
{

  public class UIMainManuComponent : UIWindowComponent
  {
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
    }

    private void OnDestroy()
    {
      _definition.Terminate();
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
  }
}