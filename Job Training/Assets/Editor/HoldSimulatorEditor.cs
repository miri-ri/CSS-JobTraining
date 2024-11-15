using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HoldSimulator))]

public class HoldSimulatorEditor : Editor
{

    private SerializedProperty MaxTraction;
    private SerializedProperty streanghtStep;
    private SerializedProperty range;
    // Start is called before the first frame update
    void OnEnable()
    {
        MaxTraction = serializedObject.FindProperty("MaxTraction");
        streanghtStep = serializedObject.FindProperty("streanghtStep");
        range = serializedObject.FindProperty("range");
    }

    // Update is called once per frame
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        HoldSimulator m = (HoldSimulator)target;

        EditorGUILayout.LabelField("The simulated hold is in opsition (" + m.row + ", " +  m.col + ")" );
        GUIStyle currentStyle = new GUIStyle();
        Color c = new Color(0f, 1f, 0f, 0.5f);
        currentStyle.normal.textColor = Color.black;
        if (!m.isSensorized)
        {
            c = Color.clear;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("This hold is not sensorized"), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }
        else
        {
            c = Color.green;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("This hold is sensorized"), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));

            EditorGUILayout.Space();
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField("To simulate the presence of a pressign hold, position the mouse on the hold cube and click with the left button.");
            EditorGUILayout.LabelField("The longer you keep the key pressed, the longher it will it pressed, the stronger the hold will be hold");
            EditorGUILayout.LabelField("To keep the Hold pressed, press also the left shift key.");

            EditorGUILayout.PropertyField(range, new GUIContent("Precision range for the simulation"));
            EditorGUILayout.PropertyField(MaxTraction, new GUIContent("Maximum traction allowed"));
            EditorGUILayout.PropertyField(streanghtStep, new GUIContent("Incremental strenght added at each iteration"));
        }


        serializedObject.ApplyModifiedProperties();
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
