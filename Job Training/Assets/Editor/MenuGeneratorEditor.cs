using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(MenuGenerator))]
public class MenuGeneratorEditor : Editor
{
    private SerializedProperty configurationname;
    private SerializedProperty themepropertyname;
    private SerializedProperty allowOnlyCompleteThemes;
    private SerializedProperty generateAtRuntime;

    private SerializedProperty GameSceneIndex;

    private List<string> confignames = new List<string>();

    private List<string> Propertynames = new List<string>();

    private SerializedProperty hastheme;

    private int indexClass;
    private int indexParameter;

    private void OnEnable()
    {
        List<GameConfiguration> possibleconfig = ReflectiveEnumerator.GetEnumerableOfType<GameConfiguration>();
        confignames = new List<string>();
        foreach (GameConfiguration g in possibleconfig)
        {
            confignames.Add(g.GetType().ToString());
        }
        generateAtRuntime = serializedObject.FindProperty("generateAtRuntime");
        configurationname = serializedObject.FindProperty("configurationname");
        themepropertyname = serializedObject.FindProperty("ThemePropertyName");
        hastheme = serializedObject.FindProperty("hastheme");
        allowOnlyCompleteThemes = serializedObject.FindProperty("allowOnlyCompleteThemes");
        GameSceneIndex = serializedObject.FindProperty("GameSceneIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.IntSlider(GameSceneIndex, 1, UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings, "Game Scene Index");

        EditorGUILayout.PropertyField(generateAtRuntime);

        indexClass = EditorGUILayout.Popup(indexClass, confignames.ToArray());
        MenuGenerator generator = (MenuGenerator)target;
        EditorGUILayout.PropertyField(hastheme);
        if (((MenuGenerator)target).hastheme)
        {
            Type t = ((MenuGenerator)target).configurationschema.GetType();
            PropertyInfo[] proplist = t.GetProperties();
            Propertynames = new List<string>();
            for (int i = 0; i < proplist.Length; i++)
            {
                Propertynames.Add(proplist[i].Name);
                if (proplist[i].Name == themepropertyname.stringValue)
                {
                    indexParameter = i;
                }
            }
            indexParameter = EditorGUILayout.Popup(indexParameter, Propertynames.ToArray());
            EditorGUILayout.PropertyField(allowOnlyCompleteThemes);
            ThemeManager.StartUp();
        }
        else
        {
            ((MenuGenerator)target).ThemePropertyName = "";
        }

        string message = ((MenuGenerator)target).generateAtRuntime ? "Check the result" : "Create the menu";

        if (GUILayout.Button(message))
        {
            ((MenuGenerator)target).configurationname = confignames.ElementAt(indexClass);
            if (Propertynames.Count > 0)
            {
                ((MenuGenerator)target).ThemePropertyName = Propertynames.ElementAt(indexParameter);
            }
            generator.generateMenu();
            if (!((MenuGenerator)target).generateAtRuntime)
            {
                generateEvents((MenuGenerator)target);
            }
        }


        string message2 = "Generate the schema for DB";
        if (GUILayout.Button(message2))
        {
            generateSchema();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void generateSchema()
    {
        MenuGenerator m = (MenuGenerator)target;
        dynamic manifest = new JObject();

        manifest.id = "/" + Application.productName.Replace(" ", "");
        manifest.type = "object";
        JObject props = new JObject();

        PropertyInfo[] info = m.configurationschema.GetType().GetProperties();
        for (int i = 0; i < info.Length; i++) {
            JObject data = new JObject();
            object[] attrs = info[i].GetCustomAttributes(true);
            data["description"] = info[i].Name;
            data["type"] = info[i].PropertyType.ToString(); //fix types
            foreach (Attribute a in attrs)
            {
                if (a.GetType().ToString() == "PropertyRename")
                {
                    PropertyRename rnm = a as PropertyRename;
                    data["description"] = rnm.easyname;
                }

                if (info[i].PropertyType == typeof(int) || info[i].PropertyType == typeof(float))
                {
                    if (a.GetType().ToString() == "PropertyRange")
                    {
                        PropertyRange r = a as PropertyRange;
                        data["minimum"] = r.min;
                        data["maximum"] = r.max;
                        data["multipleOf"] = 1;
                    }
                }

                if (info[i].PropertyType == typeof(string))
                {
                    
                    if (a.GetType().ToString() == "PropertyLimitedSet")
                    {
                        JArray Jls = new JArray();
                        PropertyLimitedSet s = a as PropertyLimitedSet;
                        foreach (string sv in s.values) {

                            Jls.Add(sv);
                        }

                        data["enum"] = Jls;
                    }
                    else if (a.GetType().ToString() == "PropertyReferenceFolder")
                    {
                        JArray Jls = new JArray();
                        PropertyReferenceFolder prf = a as PropertyReferenceFolder;
                        List<string> ls = new List<string>();
                        foreach (string f in Directory.GetFiles(Application.streamingAssetsPath + "/" + prf.folder))
                        {
                            string file = f.Split('/').Last().Split('\\').Last();
                            if (file.EndsWith(prf.extension))
                            {
                                ls.Add(file);
                            }
                        }

                        foreach (string sv in ls)
                        {
                            Jls.Add(sv);
                        }
                        data["enum"] = Jls;
                    }
                }

               

            }
            if (info[i].PropertyType == typeof(bool))
            {
                JArray arr = new JArray();
                JObject corr = new JObject();
                corr["const"] = true;
                corr["description"] = "Si";
                arr.Add(corr);
                JObject uncorr = new JObject();
                uncorr["const"] = false;
                uncorr["description"] = "No";
                arr.Add(uncorr);
                data["oneOf"] = arr;
            }

            if (info[i].PropertyType.IsEnum)
            {
                JArray Jls = new JArray();
                foreach (var v in Enum.GetValues(info[i].PropertyType))
                {
                    Jls.Add(v.ToString());
                }
                
                data["enum"] = Jls;
            }

            props[info[i].Name] = data;
            Debug.Log(info[i].Name);
            Debug.Log(info.Length + " " + i);
        }

        manifest.properties = props;

        string manifeststringfy = manifest.ToString();
        manifeststringfy.Replace("adbconst:", "const:");

        File.WriteAllText(Application.dataPath + "/Resources/DBSchema.dat", manifeststringfy);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void generateEvents(MenuGenerator target)
    {
        UnityAction<GameObject> action;

        action = new UnityAction<GameObject>(target.OnClickPlayGame);
        UnityEventTools.AddObjectPersistentListener<GameObject>(target.playgame.onClick, action, target.playgame.transform.gameObject);

        foreach (PropertyMenuBlockManager p in target.blocks)
        {
            if (p.slider != null)
            {
                action = new UnityAction<GameObject>(p.SliderEvent);
                UnityEventTools.AddObjectPersistentListener<GameObject>(p.slider.onValueChanged, action, p.slider.transform.gameObject);
            }
            if (p.textfield != null)
            {
                action = new UnityAction<GameObject>(p.InputTextEvent);
                UnityEventTools.AddObjectPersistentListener<GameObject>(p.textfield.onEndEdit, action, p.textfield.transform.gameObject);
            }
            if (p.boolfield != null)
            {
                action = new UnityAction<GameObject>(p.TogglerEvent);
                UnityEventTools.AddObjectPersistentListener<GameObject>(p.boolfield.onValueChanged, action, p.boolfield.transform.gameObject);
            }
            if (p.prevButton != null)
            {
                action = new UnityAction<GameObject>(p.PrevButtonEvent);
                UnityEventTools.AddObjectPersistentListener<GameObject>(p.prevButton.onClick, action, p.prevButton.transform.gameObject);
            }
            if (p.nextButton != null)
            {
                action = new UnityAction<GameObject>(p.NextButtonEvent);
                UnityEventTools.AddObjectPersistentListener<GameObject>(p.nextButton.onClick, action, p.nextButton.transform.gameObject);
            }
            if (p.optiondropdown != null)
            {
                if (p.Hasstringoptions)
                {
                    action = new UnityAction<GameObject>(p.StringOptionEvent);
                }
                else
                {
                    action = new UnityAction<GameObject>(p.DropdownEvent);
                }
                UnityEventTools.AddObjectPersistentListener<GameObject>(p.optiondropdown.onValueChanged, action, p.optiondropdown.transform.gameObject);
            }
        }
    }
}