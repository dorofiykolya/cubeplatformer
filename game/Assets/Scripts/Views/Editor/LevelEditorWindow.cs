using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using Utils.Editor;

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

    void OnEnable()
    {
      _factoryFold = new AnimBool(true);
      _factoryFold.valueChanged.AddListener(Repaint);
    }

    void OnGUI()
    {
      _factoryFold.target = EditorUtils.FoldoutHeader("Editor", _factoryFold.target);
      if (EditorGUILayout.BeginFadeGroup(_factoryFold.faded))
      {
        EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);
        EditorGUILayout.TextField("fgfd", "");
        EditorGUILayout.TextField("asd", "asdasd");
        EditorGUILayout.EndVertical();
      }
      EditorGUILayout.EndFadeGroup();

    }
  }
}