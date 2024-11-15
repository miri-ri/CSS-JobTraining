using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TrackerClimbingWall))]
public class ClimbingWallTrackerEditor : Editor
{

    private SerializedProperty GameWorldSize;
    private SerializedProperty inCameraSpace;
    private SerializedProperty threshold;

    private bool showTips= false;
    private string TipsHelpBoxs = "How to use it";

    private void OnEnable()
    {
        GameWorldSize = serializedObject.FindProperty("GameWorldSize");
        inCameraSpace = serializedObject.FindProperty("inCameraSpace");
        threshold = serializedObject.FindProperty("threshold");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //TrackerClimbingWall m = (TrackerClimbingWall)target;
        EditorGUILayout.LabelField("The visual for the front camera is in Camera Space?");
        EditorGUILayout.PropertyField(inCameraSpace);

        EditorGUILayout.LabelField("Determine the in-game dimension of the space:");
        EditorGUILayout.PropertyField(GameWorldSize);

        EditorGUILayout.LabelField("Set the sensibility level.");
        int min = 0;
        int max = 500;
        EditorGUILayout.IntSlider(threshold, min, max, new GUILayoutOption[0]);


        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To detect if a hold has been grasped, register to the event GraspedHold(int column, int row)", MessageType.Info);
            EditorGUILayout.HelpBox("To detect if a hold has been released, register to the event ReleasedHoldEvent(int column, int row)", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
