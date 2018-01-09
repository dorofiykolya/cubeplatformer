using Game.Components.Prototypes;
using Game.Prototypes.Elements;
using UnityEditor;
using UnityEngine;
using Utils.Editor;
using MaterialProperty = Game.Components.Prototypes.MaterialProperty;

namespace Game.ScriptableObjects.Editor
{
  [CustomEditor(typeof(MaterialProperty))]
  public class MaterialPropertyPrototypeEditor : Editor<MaterialProperty>
  {
    public override void OnInspectorGUI()
    {
      Target.Density = EditorGUILayout.FloatField("Density", Target.Density);

      EditorUtils.BeginVerticalHeader("Resistances");
      if (Target.Resistances != null)
      {
        var index = 0;
        var needBreak = false;
        foreach (var resistance in Target.Resistances)
        {
          EditorGUILayout.BeginHorizontal();
          EditorUtils.BeginVertical((index % 2) == 0);
          resistance.ElementType = (ElementType)EditorGUILayout.EnumPopup("Type", resistance.ElementType);
          resistance.Value = EditorGUILayout.FloatField("Value", resistance.Value);
          index++;
          EditorUtils.EndVertical();
          if (GUILayout.Button("X", GUILayout.Width(20f)))
          {
            ArrayUtility.Remove(ref Target.Resistances, resistance);
            needBreak = true;
          }
          EditorGUILayout.EndHorizontal();
          if (needBreak)
          {
            break;
          }
        }
      }
      if (GUILayout.Button("+", EditorStyles.miniButton))
      {
        if (Target.Resistances == null)
        {
          Target.Resistances = new MaterialResistance[1];
          Target.Resistances[0] = new MaterialResistance();
        }
        else
        {
          ArrayUtility.Add(ref Target.Resistances, new MaterialResistance());
        }
      }
      EditorUtils.EndVerticalHeader();

      EditorGUILayout.Separator();

      EditorUtils.BeginVerticalHeader("Conductivities");
      if (Target.Conductivities != null)
      {
        var index = 0;
        var needBreak = false;
        foreach (var resistance in Target.Conductivities)
        {
          EditorGUILayout.BeginHorizontal();
          EditorUtils.BeginVertical((index % 2) == 0);
          resistance.ElementType = (ElementType)EditorGUILayout.EnumPopup("Type", resistance.ElementType);
          resistance.Value = EditorGUILayout.FloatField("Value", resistance.Value);
          index++;
          EditorUtils.EndVertical();
          if (GUILayout.Button("X", GUILayout.Width(20f)))
          {
            ArrayUtility.Remove(ref Target.Conductivities, resistance);
            needBreak = true;
          }
          EditorGUILayout.EndHorizontal();
          if (needBreak)
          {
            break;
          }
        }
      }
      if (GUILayout.Button("+", EditorStyles.miniButton))
      {
        if (Target.Conductivities == null)
        {
          Target.Conductivities = new MaterialConductivity[1];
          Target.Conductivities[0] = new MaterialConductivity();
        }
        else
        {
          ArrayUtility.Add(ref Target.Conductivities, new MaterialConductivity());
        }
      }
      EditorUtils.EndVerticalHeader();
      EditorUtility.SetDirty(Target);
    }
  }
}
