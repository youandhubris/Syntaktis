using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using System;
using System.IO;

namespace Hubris.EditorUtility
{
    /// <summary>
    /// </summary>
    public static class Screenshot
    {
        [MenuItem("Hubris/Screenshot/Game %#q", false, 51)]
        static void ScreenshotGame()
        {
            string path = Application.dataPath + "/StreamingAssets/";
            string filename = GetFilename("Game");

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    Debug.unityLogger.LogError(typeof(SaveOnPlay).ToString(), e);
                    throw;
                }
            }

            ScreenCapture.CaptureScreenshot(path + filename);
            Debug.unityLogger.Log(typeof(SaveOnPlay).ToString(), "Saved: " + path + filename);
        }

        [MenuItem("Hubris/Screenshot/Scene %#w", false, 52)]
        static void ScreenshotSceneNoGizmos()
        {
            ScreenshotScene(false);
        }

        [MenuItem("Hubris/Screenshot/Scene + Gizmos %#e", false, 53)]
        static void ScreenshotSceneGizmos()
        {
            ScreenshotScene(true);
        }

        static void ScreenshotScene(bool gizmos)
        {
            RenderTexture renderTexture = Camera.current.activeTexture;
            if (!gizmos) RenderTexture.active = renderTexture;

            Texture2D texture2d = new Texture2D(renderTexture.width, renderTexture.height);
            texture2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2d.Apply();

            byte[] bytes = texture2d.EncodeToPNG();
            string path = Application.dataPath + "/StreamingAssets/";
            string filename = GetFilename(gizmos ? "SceneGizmos" : "Scene");

            if (SaveScreenshot(bytes, path, filename))
            {
                RenderTexture.active = null;
                MonoBehaviour.DestroyImmediate(renderTexture);
                MonoBehaviour.DestroyImmediate(texture2d);

                Debug.unityLogger.Log(typeof(SaveOnPlay).ToString(), "Saved: " + path + filename);
            }
            else Debug.unityLogger.LogError(typeof(SaveOnPlay).ToString(), "Not saved.");

        }

        static string GetFilename(string type)
        {
            return EditorSceneManager.GetActiveScene().name + "-" + type + "-" + Khronos.GetDateCompact(DateTime.Now) + ".png";
        }

        static bool SaveScreenshot(byte [] bytes, string path, string filename)
        {
            bool saved = false;

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    Debug.unityLogger.LogError(typeof(SaveOnPlay).ToString(), e);
                    throw;
                }
            }

            try
            {
                File.WriteAllBytes(path + filename, bytes);
                saved = true;
            }
            catch (Exception e)
            {
                Debug.unityLogger.LogError(typeof(SaveOnPlay).ToString(), e);
                throw;
            }

            return saved;
        }

    }
}