using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;
using Utils.Editor;

namespace Game.Components.Levels.Editor
{
  [CustomEditor(typeof(LevelEnvironmentItemComponent)), CanEditMultipleObjects]
  public class LevelEnvironmentItemComponentEditor : Editor<LevelEnvironmentItemComponent>
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUILayout.Space();
      if (Targets.Length == 1)
      {
        EditorUtils.BeginVerticalHeader("Required Cells");
        var cellSize = LevelEnvironmentComponent.CellSize;
        var rect = EditorGUILayout.GetControlRect(false, 64f);
        rect.width = 16 * 3f;
        for (int x = 0; x < cellSize.x; x++)
        {
          for (int y = 0; y < cellSize.y; y++)
          {
            var pos = rect;
            pos.x += 16f * x;
            pos.y += 16f * y;
            pos.width = pos.height = 16f;
            EditorUtils.PushColor();
            var contains = Contains(x, y);
            if (contains)
            {
              GUI.color = Color.green;
            }

            if (GUI.Button(pos, GUIContent.none, EditorStyles.miniButton))
            {
              if (contains) Remove(x, y);
              else Add(x, y);
              EditorUtility.SetDirty(target);
            }

            EditorUtils.PopColor();
          }
        }

        EditorUtils.EndVerticalHeader();
      }
    }

    private bool Contains(int x, int y)
    {
      return Target.RequireCells != null && Target.RequireCells.Any(c => c.x == x && c.y == y);
    }

    private void Add(int x, int y)
    {
      if (Target.RequireCells == null)
      {
        Target.RequireCells = new Point[0];
      }

      var list = new HashSet<Point>(Target.RequireCells);
      list.Add(new Point(x, y));
      Target.RequireCells = list.ToArray();
    }

    private void Remove(int x, int y)
    {
      if (Target.RequireCells != null && Target.RequireCells.Length != 0)
      {
        var list = Target.RequireCells.Distinct().ToList();
        list.Remove(new Point(x, y));
        Target.RequireCells = list.ToArray();
      }
    }
  }
}
