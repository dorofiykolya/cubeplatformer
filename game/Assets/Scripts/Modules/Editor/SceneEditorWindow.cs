using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityEditor
{
  public class SceneEditorWindow : EditorWindow
  {
    private static List<string> _scenes;

    [MenuItem("Window/Scenes")]
    private static void Open()
    {
      GetWindow<SceneEditorWindow>("Scenes").Show(true);
    }

    private void OnGUI()
    {
      if (_scenes == null)
      {
        _scenes = new List<string>(Directory.GetFiles("Assets/Scenes").Where(f => Path.GetExtension(f) == ".unity").OrderBy(s => s));
      }
      foreach (var scene in _scenes)
      {
        if (GUILayout.Button(Path.GetFileNameWithoutExtension(scene), EditorStyles.miniButton))
        {
          EditorSceneManager.OpenScene(scene);
          break;
        }
      }
    }
  }
}
