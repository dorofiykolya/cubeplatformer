using UnityEditor;

namespace Game.Components.Editor
{
  [CustomEditor(typeof(CharacterController))]
  public class CharacterControllerEditor : Editor<CharacterController>
  {
    public override void OnInspectorGUI()
    {
      Target.Trigger = (CharacterTrigger)EditorGUILayout.EnumPopup(Target.Trigger);
    }
  }
}
