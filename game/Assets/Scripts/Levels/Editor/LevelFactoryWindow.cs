using Game.Levels;
using UnityEngine;
using UnityEditor;
using Utils.Editor;

namespace Game.Editor
{
  public class LevelFactoryWindow : EditorWindow
  {
    [MenuItem("Game/LevelFactory")]
    public static void Open()
    {
      var window = GetWindow<LevelFactoryWindow>("LevelFactory");
      window.Show(true);
    }

    private static LevelSize _size = new LevelSize { X = 18, Y = 10, Z = 1 };
    private static LevelCoordinateConverter _coordinateConverter;
    private static LevelFactoryOptions _options;

    private void OnGUI()
    {
      if (_options == null) _options = new LevelFactoryOptions();

      EditorUtils.Header("Factory");
      EditorGUILayout.BeginVertical(EditorUtils.Styles.ProgressBarBack);

      EditorUtils.Header("Size", -2f);
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
      _coordinateConverter = EditorGUILayout.ObjectField(_coordinateConverter, typeof(LevelCoordinateConverter), false) as LevelCoordinateConverter;
      EditorGUILayout.Space();

      foreach (var field in InspectAttribute<LevelFactoryOptions>.Fields)
      {
        var value = field.GetValue(_options);
        field.SetValue(_options, EditorGUILayoutProperty.DrawProperty(field.Name, value, field.FieldType));
      }

      if (GUILayout.Button("Create"))
      {
        if (_coordinateConverter == null)
        {
          EditorUtility.DisplayDialog("coordinate converter must been initialized", "null", "Close");
        }
        else if (_size.X <= 0 || _size.Y <= 0 || _size.Z <= 0)
        {
          EditorUtility.DisplayDialog("Size Invalid Values", _size.ToString(), "Close");
        }
        else
        {
          var level = LevelFactory.Create(_size, _coordinateConverter);
          Selection.activeGameObject = level.gameObject;
          LevelEditorWindow.Open(level);
          Close();
        }
      }
      EditorGUILayout.EndVertical();
    }
  }
}