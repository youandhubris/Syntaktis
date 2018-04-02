using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using Hubris;

namespace Hubris.EditorUtility
{
    /// <summary>
    /// Saves active scene on Play.
    /// </summary>
    [InitializeOnLoadAttribute]
    public static class SaveOnPlay
    {
        static SaveOnPlay()
        {
            EditorApplication.playModeStateChanged += DoAutoSaveOnPlay;
        }

        private static void DoAutoSaveOnPlay(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:

                    Scene activeScene = EditorSceneManager.GetActiveScene();

                    if (activeScene.isDirty)
                    {
                        bool savedScene = EditorSceneManager.SaveScene(activeScene);
                        if (savedScene) Debug.unityLogger.Log(typeof(SaveOnPlay).ToString(), "Active scene was saved.");

                        AssetDatabase.SaveAssets();
                    }
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// This script creates a new window in the editor with a auto-save function. 
    /// It's saving your current scene with a specified interval from 1 minute to 60 minutes.
    /// The config window is on Window/Hubris/Auto-Save.
    /// </summary>
    public class AutoSaveOnEditor : EditorWindow
    {
        private bool autoSaveScene = true;
        private int autoSaveInterval = 5;
        private bool showLog = true;
        private int lastAutoSaveTime = Khronos.UnixTimeNow();

        [MenuItem("Hubris/Auto-Save", false, 1)]
        static void Init()
        {
            var window = (AutoSaveOnEditor)GetWindow(typeof(AutoSaveOnEditor));
            var content = new GUIContent
            {
                text = "Auto-Save",
                image = Resources.Load("Hubris-Favicon") as Texture2D
            };

            window.titleContent = content;
        }

        public void Awake()
        {
            EditorApplication.update += Update;
        }

        public void OnDestroy()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (autoSaveScene && CheckTimeInterval())
            {
                SaveScene();
                lastAutoSaveTime = Khronos.UnixTimeNow();
            }
         }

        bool CheckTimeInterval()
        {
            if (Khronos.UnixTimeNow() >= (lastAutoSaveTime + autoSaveInterval * 60)) return true;
            else return false;
        }

        void SaveScene()
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();

            if (activeScene.isDirty)
            {
                bool savedScene = EditorSceneManager.SaveScene(activeScene);
                if (savedScene) Debug.unityLogger.Log(GetType().ToString(), "Active scene was auto-saved at " + Khronos.UnixTimeNow());

                AssetDatabase.SaveAssets();
            }
        }

        void OnGUI()
        {
            GUILayout.Label("Info:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Saving:", EditorSceneManager.GetActiveScene().name);
            EditorGUILayout.Space();

            GUILayout.Label("Options:", EditorStyles.boldLabel);
            autoSaveScene = EditorGUILayout.BeginToggleGroup("Auto-Save", autoSaveScene);
            showLog = EditorGUILayout.BeginToggleGroup("Log", showLog);
            autoSaveInterval = EditorGUILayout.IntSlider("Interval (minutes)", autoSaveInterval, 1, 60);
        }
    }
}