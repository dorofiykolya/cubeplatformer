// /************************************************************
// *                                                           *
// *   Mobile Touch Camera                                     *
// *                                                           *
// *   Created 2015 by BitBender Games                         *
// *                                                           *
// *   bitbendergames@gmail.com                                *
// *                                                           *
// ************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BitBenderGames {

  [CustomEditor(typeof(MobileTouchCamera))]
  public class MobileTouchCameraEditor : CustomInspector {

    public override void OnInspectorGUI() {

      MobileTouchCamera mobileTouchCamera = (MobileTouchCamera)target;

      DrawPropertyField("m_Script");

      string camAxesError = mobileTouchCamera.CheckCameraAxesErrors();
      bool isAxesValid = string.IsNullOrEmpty(camAxesError);
      DrawPropertyField("cameraAxes", isAxesValid);
      DrawErrorLine(camAxesError, Color.red);
      GUI.enabled = mobileTouchCamera.GetComponent<Camera>().orthographic == false;
      DrawPropertyField("perspectiveZoomMode");
      GUI.enabled = true;
      bool isZoomValid = mobileTouchCamera.CamZoomMax >= mobileTouchCamera.CamZoomMin;
      DrawPropertyField("camZoomMin", isZoomValid);
      DrawPropertyField("camZoomMax", isZoomValid);
      if (isZoomValid == false) {
        DrawErrorLine("Cam Zoom Max must be bigger than Cam Zoom Min", Color.red);
      }
      DrawPropertyField("camOverzoomMargin");
      DrawPropertyField("camOverdragMargin");

      #region boundary
      SerializedProperty serializedPropertyBoundaryMin = serializedObject.FindProperty("boundaryMin");
      Vector2 vector2BoundaryMin = serializedPropertyBoundaryMin.vector2Value;

      SerializedProperty serializedPropertyBoundaryMax = serializedObject.FindProperty("boundaryMax");
      Vector2 vector2BoundaryMax = serializedPropertyBoundaryMax.vector2Value;

      EditorGUILayout.LabelField(new GUIContent("Boundary", "These values define the scrolling borders for the camera. The camera will not scroll further than defined here. The boundary is drawn as yellow rectangular gizmo in the scene-view when the camera is selected."), EditorStyles.boldLabel);

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Top", GUILayout.Width(sizeLabel));
      GUILayout.FlexibleSpace();
      GUILayout.FlexibleSpace();
      vector2BoundaryMax.y = EditorGUILayout.FloatField(vector2BoundaryMax.y, GUILayout.Width(sizeFloatInput));
      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();

      Draw2FloatFields("Left/Right", ref vector2BoundaryMin.x, ref vector2BoundaryMax.x);

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Bottom", GUILayout.Width(sizeLabel));
      GUILayout.FlexibleSpace();
      GUILayout.FlexibleSpace();
      vector2BoundaryMin.y = EditorGUILayout.FloatField(vector2BoundaryMin.y, GUILayout.Width(sizeFloatInput));
      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();

      serializedPropertyBoundaryMin.vector2Value = vector2BoundaryMin;
      serializedPropertyBoundaryMax.vector2Value = vector2BoundaryMax;
      #endregion

      DrawPropertyField("camFollowFactor");
      DrawPropertyField("moveCamFollowFactor");

      #region auto scroll damp
      AutoScrollDampMode selectedDampMode = (AutoScrollDampMode)serializedObject.FindProperty("autoScrollDampMode").enumValueIndex;
      if (selectedDampMode == AutoScrollDampMode.DEFAULT && mobileTouchCamera.AutoScrollDamp != 300) {
        serializedObject.FindProperty("autoScrollDampMode").enumValueIndex = (int)AutoScrollDampMode.CUSTOM; //Set selected mode to custom in case it was set to default but the damp wasn't the default value. This may happen for users that have changed the damp and upgraded from an older version of the asset.
        selectedDampMode = AutoScrollDampMode.CUSTOM;
      }
      DrawPropertyField("autoScrollDampMode");
      if (selectedDampMode == AutoScrollDampMode.CUSTOM) {
        DrawPropertyField("autoScrollDamp", true, true, subSettingsInset);
        DrawPropertyField("autoScrollDampCurve", true, true, subSettingsInset);
      }
      #endregion

      DrawPropertyField("groundLevelOffset");
      DrawPropertyField("enableRotation");
      DrawPropertyField("enableTilt");
      if (mobileTouchCamera.EnableTilt == true) {
        const float minTiltErrorAngle = 10;
        const float minTiltWarningAngle = 40;
        DrawPropertyField("tiltAngleMin", mobileTouchCamera.TiltAngleMin >= minTiltErrorAngle, mobileTouchCamera.TiltAngleMin >= minTiltWarningAngle, subSettingsInset);
        if (mobileTouchCamera.TiltAngleMin < minTiltErrorAngle) {
          DrawErrorLine("Error: The minimum tilt angle must not be lower than " + minTiltErrorAngle + ". Otherwise the camera computation is guaranteed to become instable.", Color.red);
        } else if (mobileTouchCamera.TiltAngleMin < minTiltWarningAngle) {
          DrawErrorLine("Warning: The minimum tilt angle should not be lower than " + minTiltWarningAngle + ". Otherwise the camera computations may become instable.", Color.yellow);
        }
        const float maxTiltErrorAngle = 90;
        DrawPropertyField("tiltAngleMax", mobileTouchCamera.TiltAngleMax <= maxTiltErrorAngle, true, subSettingsInset);
        if (mobileTouchCamera.TiltAngleMax > maxTiltErrorAngle) {
          DrawErrorLine("The maximum tilt angle must not be higher than " + maxTiltErrorAngle + ". Otherwise the camera computation may become instable.", Color.red);
        }
        if (mobileTouchCamera.TiltAngleMax < mobileTouchCamera.TiltAngleMin) {
          DrawErrorLine("Tilt Angle Max must be bigger than Tilt Angle Min", Color.red);
        }
      }

      DrawPropertyField("OnPickItem");
      DrawPropertyField("OnPickItem2D");
      DrawPropertyField("OnPickItemDoubleClick");
      DrawPropertyField("OnPickItem2DDoubleClick");

      DrawPropertyField("expertModeEnabled");
      SerializedProperty serializedPropertyExpertMode = serializedObject.FindProperty("expertModeEnabled");
      if(serializedPropertyExpertMode.boolValue == true) {
        DrawPropertyField("zoomBackSpringFactor");
        DrawPropertyField("dragBackSpringFactor");
        DrawPropertyField("autoScrollVelocityMax");
        DrawPropertyField("dampFactorTimeMultiplier");
        DrawPropertyField("isPinchModeExclusive");
        DrawPropertyField("customZoomSensitivity");
        DrawPropertyField("terrainCollider");

        DrawPropertyField("rotationDetectionDeltaThreshold");
        DrawPropertyField("rotationMinPinchDistance");
        DrawPropertyField("rotationLockThreshold");

        DrawPropertyField("pinchModeDetectionMoveTreshold");
        DrawPropertyField("pinchTiltModeThreshold");
        DrawPropertyField("pinchTiltSpeed");
      }

      if (GUI.changed) {
        serializedObject.ApplyModifiedProperties();

        //Detect modified properties.
        AutoScrollDampMode dampModeAfterApply = (AutoScrollDampMode)serializedObject.FindProperty("autoScrollDampMode").enumValueIndex;
        if (selectedDampMode != dampModeAfterApply) {
          OnScrollDampModeChanged(dampModeAfterApply);
        }
      }
    }

    private void OnScrollDampModeChanged(AutoScrollDampMode dampMode) {

      SerializedProperty serializedScrollDamp = serializedObject.FindProperty("autoScrollDamp");
      SerializedProperty serializedScrollDampCurve = serializedObject.FindProperty("autoScrollDampCurve");
      switch (dampMode) {
        case AutoScrollDampMode.DEFAULT:
          serializedScrollDamp.floatValue = 300;
          serializedScrollDampCurve.animationCurveValue = new AnimationCurve(new Keyframe(0, 1, 0, 0), new Keyframe(0.7f, 0.9f, -0.5f, -0.5f), new Keyframe(1, 0.01f, -0.85f, -0.85f));
          break;
        case AutoScrollDampMode.SLOW_FADE_OUT:
          serializedScrollDamp.floatValue = 150;
          serializedScrollDampCurve.animationCurveValue = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0.01f, -1, -1));
          break;
      }
      if (dampMode != AutoScrollDampMode.CUSTOM) {
        serializedObject.ApplyModifiedProperties();
      }
    }

    private void Draw2FloatFields(string caption, ref float valueA, ref float valueB) {

      EditorGUILayout.BeginHorizontal();
      GUILayout.Label(caption, GUILayout.Width(sizeLabel));
      GUILayout.FlexibleSpace();
      valueA = EditorGUILayout.FloatField(valueA, GUILayout.Width(sizeFloatInput));
      GUILayout.FlexibleSpace();
      valueB = EditorGUILayout.FloatField(valueB, GUILayout.Width(sizeFloatInput));
      EditorGUILayout.EndHorizontal();

    }
  }
}
