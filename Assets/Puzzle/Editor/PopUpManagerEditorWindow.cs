using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Puzzle.UI
{
    public class PopUpManagerEditorWindow : EditorWindow
    {
        private const string fileDirectory = "Assets/Runtime";
        private const string filePath = "Assets/Runtime/PopUpPath.cs";
        private const string popUpDirectory = "Assets/Resources/PopUp";
        private const string popUpFolder = "PopUp";
        private const string pattern = "[^a-zA-Z0-9]";
        private const float nameWidth = 0.3f;
        private const float pathWidth = 0.3f;
        private const float activeWidth = 0.1f;

        private List<PopUp> _popUps;
        private Vector2 scrollPosition;

        [MenuItem("Tools/PopUp/Manager %&#M")]
        public static void CreateManager()
        {
            GetWindow<PopUpManagerEditorWindow>("Pop Up Manager").Show();
        }

        [MenuItem("Tools/PopUp/Resolve %&#R")]
        public static void Resolve()
        {
            ResolvePath();
            var popUps = LoadAssets();
            ResolveAssets(popUps);
            Assets(false, popUps);
            GenerateAssets(popUps);
        }
        
        private void Awake()
        {
            ResolvePath();
            _popUps = LoadAssets();
        }

        private static void ResolvePath()
        {
            if (!Directory.Exists(popUpDirectory))
            {
                Directory.CreateDirectory(popUpDirectory);
            }

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
        }
        
        private static List<PopUp> LoadAssets()
        {
            var popUps = new List<PopUp>();
            Debug.Log(popUpDirectory);
            var assets = Resources.LoadAll(popUpFolder);
            foreach (var asset in assets)
            {
                if (asset is GameObject gameObject)
                {
                    var popUp = gameObject.GetComponent<PopUp>();
                    if (popUp != null) popUps.Add(popUp);
                }
            }

            return popUps;
        }

        private static void GenerateAssets(List<PopUp> popUps)
        {
            var text = "public class PopUpPath \n{\n";
            foreach (var popUp in popUps)
            {
                var tempText = "\tpublic const string POP_UP_" + Regex.Replace(popUp.gameObject.name, pattern, "_").ToUpper() + $" = \"{popUpFolder}/{popUp.gameObject.name}\";\n";
                text += tempText;
            }

            text += "}";

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(text);
            }

            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.Default);
        }

        private static void ResolveAssets(List<PopUp> popUps)
        {
            foreach (var popUp in popUps)
            {
                popUp.path = $"{popUpFolder}/{popUp.gameObject.name}";
                popUp.Resolve();
                EditorUtility.SetDirty(popUp.gameObject);
            }

            AssetDatabase.SaveAssets();
            foreach (var popUp in popUps)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(popUp.gameObject), ImportAssetOptions.Default);
            }
        }

        private static void Assets(bool enable, List<PopUp> popUps)
        {
            foreach (var popUp in popUps)
            {
                popUp.gameObject.SetActive(enable);
                EditorUtility.SetDirty(popUp.gameObject);
            }

            AssetDatabase.SaveAssets();
            foreach (var popUp in popUps)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(popUp.gameObject), ImportAssetOptions.Default);
            }
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload"))
            {
                _popUps = LoadAssets();
            }

            if (GUILayout.Button("Resolve"))
            {
                ResolveAssets(_popUps);
                Repaint();
            }

            if (GUILayout.Button("Disable all"))
            {
                Assets(false, _popUps);
                Repaint();
            }

            if (GUILayout.Button("Generate C# file"))
            {
                GenerateAssets(_popUps);
            }
            
            if (GUILayout.Button("Execute all"))
            {
                Resolve();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            var color = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField("name", GUILayout.Width(nameWidth * position.width));
            EditorGUILayout.LabelField("path", GUILayout.Width(pathWidth * position.width));
            EditorGUILayout.LabelField("active", GUILayout.Width(activeWidth * position.width));
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = color;
            foreach (var popUp in _popUps)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField(popUp.gameObject.name, GUILayout.Width(nameWidth * position.width));
                EditorGUILayout.LabelField(popUp.path, GUILayout.Width(pathWidth * position.width));
                var toggle = EditorGUILayout.Toggle(popUp.gameObject.activeSelf, GUILayout.Width(activeWidth * position.width));
                if (toggle != popUp.gameObject.activeSelf)
                {
                    popUp.gameObject.SetActive(toggle);
                    EditorUtility.SetDirty(popUp.gameObject);
                    AssetDatabase.SaveAssets();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}