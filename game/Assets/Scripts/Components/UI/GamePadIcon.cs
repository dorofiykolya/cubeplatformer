using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components
{
  [ExecuteInEditMode]
  [RequireComponent(typeof(Image))]
  public class GamePadIcon : MonoBehaviour
  {
    public enum GamePadButton
    {
      None,
      Targets,
      TargetLeft,
      TargetRight,
      TargetUp,
      TargetDown,
      ButtonXTop,
      ButtonYLeft,
      ButtonABottom,
      ButtonBRight,
      ButtonMenuLeft,
      ButtonMenuRight
    }

    public Image Image;
    public GamePadButtonIconPreset Preset;
    public GamePadButton Button;

    private void Awake()
    {
      if (Image == null) Image = GetComponent<Image>();
      Image.sprite = GetSprite(Button);
    }

    [Conditional("UNITY_EDITOR")]
    private void Update()
    {
      if (Image)
      {
        Image.sprite = GetSprite(Button);
      }
    }

    private Sprite GetSprite(GamePadButton button)
    {
      if (Preset != null)
      {
        switch (button)
        {
          case GamePadButton.Targets: return Preset.TargetButtons;
          case GamePadButton.TargetLeft: return Preset.TargetLeft;
          case GamePadButton.TargetRight: return Preset.TargetRight;
          case GamePadButton.TargetUp: return Preset.TargetUp;
          case GamePadButton.TargetDown: return Preset.TargetDown;
          case GamePadButton.ButtonABottom: return Preset.ButtonABottom;
          case GamePadButton.ButtonBRight: return Preset.ButtonBRight;
          case GamePadButton.ButtonXTop: return Preset.ButtonXTop;
          case GamePadButton.ButtonYLeft: return Preset.ButtonYLeft;
          case GamePadButton.ButtonMenuLeft: return Preset.ButtonMenuLeft;
          case GamePadButton.ButtonMenuRight: return Preset.ButtonMenuRight;
        }
      }

      return null;
    }
  }
}
