using UnityEditor;

namespace Game.Components.Editor
{
  [CustomEditor(typeof(CharacterAnimatorController))]
  public class CharacterControllerEditor : Editor<CharacterAnimatorController>
  {
    public override void OnInspectorGUI()
    {
      Target.Trigger = (CharacterTrigger)EditorGUILayout.EnumPopup(Target.Trigger);
    }
  }
}
