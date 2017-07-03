using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using Utils.Editor;
using System.Linq;
using System;

namespace Game.Views.Editor
{
  public class LevelEditorWindow : EditorWindow
  {
    public static void Open(LevelComponent level)
    {
      var window = GetWindow<LevelEditorWindow>("LevelEditor");
      window._level = level;
      window.Show(true);
    }

    [MenuItem("Game/LevelEditor")]
    public static void OpenSelected()
    {
      if (OpenSelectedValidate())
      {
        Open(Selection.activeGameObject.GetComponent<LevelComponent>() ?? Selection.activeGameObject.GetComponentInParent<LevelComponent>());
      }
    }

    [MenuItem("Game/LevelEditor", true)]
    public static bool OpenSelectedValidate()
    {
      return Selection.activeGameObject && (Selection.activeGameObject.GetComponent<LevelComponent>() || Selection.activeGameObject.GetComponentInParent<LevelComponent>());
    }

    private static AnimBool _factoryFold = new AnimBool(false);
    private LevelComponent _level;
    private int _currentZ;

    void OnEnable()
    {
      _factoryFold = new AnimBool(true);
      _factoryFold.valueChanged.AddListener(Repaint);
    }

    void OnGUI()
    {
      if(_level == null) Close();
      _factoryFold.target = EditorUtils.FoldoutHeader("Editor", _factoryFold.target);
      if (EditorGUILayout.BeginFadeGroup(_factoryFold.faded))
      {
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        EditorUtils.Header("Grid");
        _currentZ = EditorGUILayout.IntSlider(_currentZ, 0, _level.Size.Z - 1);

        EditorGUILayout.BeginVertical();
        for(var y = _level.Size.Y - 1; y >= 0; y--)
        {
          EditorGUILayout.BeginHorizontal();
          for(var x = 0; x < _level.Size.X; x++)
          {
            EditorUtils.PushColor();
            var button = new GUIContent(" ");
            button.tooltip = string.Format("{0}:{1}", x, y);
            var cell = _level[x, y, _currentZ];
            var cellGameObject = cell.gameObject;
            if(Selection.gameObjects.Contains(cellGameObject)) GUI.color = Color.green;
            else
            {
              if(Array.IndexOf(new CellType[]{CellType.Block, CellType.HLadr, CellType.Ladder, CellType.RopeBar, CellType.Solid, CellType.Trap}, cell.CellType) != -1 || cell.CellType == CellType.Block)
              {
                GUI.color = new Color(.25f, .25f, .25f);
              }
            }
            if(GUILayout.Button(button, EditorUtils.Styles.minibutton, GUILayout.Width(10f), GUILayout.Height(10f)))
            {
              if(Event.current.command)
              {
                var selectList = Selection.gameObjects.ToList();
                if(!selectList.Contains(cellGameObject))
                {
                  selectList.Add(cellGameObject);
                  Selection.objects = selectList.ToArray();
                }
              }
              else
              {
                Selection.activeGameObject = cellGameObject;
              }
            }
            EditorUtils.PopColor();
          }
          EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
      }
      EditorGUILayout.EndFadeGroup();
    }
  }
}