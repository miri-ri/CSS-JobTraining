using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomClimbingWallManager))]
public class MagicRoomClimbingWallEditor : Editor
{

    private bool showTips;
    private bool showHolds;
    private string TipsHelpBoxs = "Show Tips on the usage";
    private string HoldDataBoxs = "Show Hold information";

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomClimbingWallManager m = (MagicRoomClimbingWallManager)target;

        EditorGUILayout.LabelField("Climbing wall traction module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        if (m.isClimbingWallActive == true)
        {
            c = new Color(0f, 1f, 0f, 0.5f);
            currentStyle.normal.textColor = Color.black;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("The climbing wall is active and runnig."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
            EditorGUILayout.LabelField(new GUIContent("The climbing has " + m.wallParams.numCol + " columns and " + m.wallParams.numRow + " rows"));
            EditorGUILayout.LabelField(new GUIContent("The climbing has a width of " + m.wallParams.wallWidth + " cm and a height of " + m.wallParams.wallHeight + " cm"));
            EditorGUILayout.LabelField(new GUIContent("The distance between the holds slot is  (" + m.wallParams.distanceX + ", " + m.wallParams.distanceY + " cm"));
            EditorGUILayout.LabelField(new GUIContent("The initial shift is  (" + m.wallParams.initialshiftX + ", " + m.wallParams.initialshiftY + " cm"));
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
            EditorGUILayout.LabelField(new GUIContent("There are " + m.holds.Count + " holds active"));
            showHolds = EditorGUILayout.Foldout(showHolds, HoldDataBoxs);
            if (showHolds)
            {
                foreach (HoldData d in m.holds)
                {
                    EditorGUILayout.LabelField(new GUIContent("Hold (" + d.row + ", " + d.column + "): " + (d.isSensorized ? "is sensorized" : "is a simple hold")));
                }
            }
        }
        else
        {
            c = Color.red;
            currentStyle.normal.textColor = Color.black;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("Server not found.The simulator is turned on."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

       showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To register and receive the skeeltons use the action Skeletons", MessageType.Info);
            EditorGUILayout.HelpBox("To detect the server use the isClimbingWallActive property (true is working, false is unconnected, null is server not found", MessageType.Info);
            EditorGUILayout.HelpBox("To start the automatic information flood (Highly suggested) from the server use the function StartStreamingHolds(...), where the parameter represent the time interval between two samplesof the holds.", MessageType.Info);
            EditorGUILayout.HelpBox("To stop the automatic information flood (Highly suggested) from the server use the function StopStreamingHolds()", MessageType.Info);
            EditorGUILayout.HelpBox("To read the players' position at command (Highly disouraged) use the function GetData(..., ...) where parameters represent the column and row of the hold to inquiry", MessageType.Info);
            GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
            EditorGUILayout.HelpBox("Unless very peculiar needs, it is sugested to use the HoldManager class which provides easyer functions  and is already managing the transformation between the simulator and the Magic Room.", MessageType.Warning);
            GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
            EditorGUILayout.HelpBox("In case you see the server connected and the simulator active, the most common cause is that you registered for the stream of skeletons before the sevr rsponded with its state. Please consider to postpone the registration to the skeletons only when you need it(typically in the game scene after the menu scene", MessageType.Warning);
        }
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
