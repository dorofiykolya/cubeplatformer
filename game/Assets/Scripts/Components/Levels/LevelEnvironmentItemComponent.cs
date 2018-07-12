using UnityEngine;
using Utils;

namespace Game.Components.Levels
{
  public class LevelEnvironmentItemComponent : MonoBehaviour
  {
    [Header("Cells")]
    public Point[] RequireCells;

    [Header("Available Connections")]
    public LevelEnvironmentComponent[] Right;
    public LevelEnvironmentComponent[] Left;
    public LevelEnvironmentComponent[] Top;
    public LevelEnvironmentComponent[] Bottom;
  }
}
