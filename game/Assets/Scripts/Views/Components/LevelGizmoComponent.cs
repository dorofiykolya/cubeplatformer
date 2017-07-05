﻿using UnityEngine;

namespace Game.Views.Components
{
  [RequireComponent(typeof(LevelComponent)), ExecuteInEditMode]
  public class LevelGizmoComponent : MonoBehaviour
  {
    public bool ShowGizmosSelectedGrid = true;
    public bool ShowGizmosWireGrid = true;
    public bool ShowGizmosGrid = true;
  }
}