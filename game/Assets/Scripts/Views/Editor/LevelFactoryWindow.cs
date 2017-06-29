using UnityEngine;
using UnityEditor;
using Utils.Editor;

namespace Game.Views.Editor
{
  public class LevelFactoryWindow : EditorWindow
  {
    [MenuItem("Game/LevelFactory")]
    public static void Open()
    {
      var window = GetWindow<LevelFactoryWindow>("LevelFactory");
      window.Show(true);
    }

    private static LevelSize _size = new LevelSize { X = 20, Y = 20, Z = 1 };
    private static LevelCoordinateConverter _coordinateConverter;

    private void OnGUI()
    {
      EditorUtils.Header("Factory");
      EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);

      EditorUtils.Header("Size", -2f, false);
      EditorGUILayout.BeginHorizontal(EditorUtils.Styles.ProgressBarBack);

      EditorUtils.PushColor();
      if (_size.X <= 0) GUI.color = Color.red;
      GUILayout.Label("X", GUILayout.Width(12f));
      _size.X = EditorGUILayout.IntField(_size.X);
      EditorUtils.PopColor();

      EditorUtils.PushColor();
      if (_size.Y <= 0) GUI.color = Color.red;
      GUILayout.Label("Y", GUILayout.Width(12f));
      _size.Y = EditorGUILayout.IntField(_size.Y);
      EditorUtils.PopColor();

      EditorUtils.PushColor();
      if (_size.Z <= 0) GUI.color = Color.red;
      GUILayout.Label("Z", GUILayout.Width(12f));
      _size.Z = EditorGUILayout.IntField(_size.Z);
      EditorUtils.PopColor();

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();
      if (GUILayout.Button("Create"))
      {
        if (_size.X <= 0 || _size.Y <= 0 || _size.Z <= 0)
        {
          EditorUtility.DisplayDialog("Size Invalid Values", _size.ToString(), "Close");
        }
        else
        {
          var level = LevelFactory.Create(_size);
          Selection.activeGameObject = level.gameObject;
          LevelEditorWindow.Open(level);
          Close();
        }
      }
      EditorGUILayout.EndVertical();
    }
  }
}