using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using TapjoyUnity.Internal;

namespace TapjoyEditor {

  public static class TapjoySettingsEditor {
    private const string TAPJOY_DIRECTORY = "TapjoySDK";
    private const string TAPJOY_SETTINGS_DIRECTORY = "Assets/TapjoySDK/";
    private const string ASSETS_DIRECTORY = "Assets";
    
    public static TapjoySettings CreateEmptySettings() {
#if DEBUG
      Debug.Log("TapjoySettings.CreateEmptySettings()");
#endif
      TapjoySettings settings = (ScriptableObject.CreateInstance(typeof(TapjoySettings)) as TapjoySettings);
      try {
        Debug.Log("Creating new Settings.asset");
        if (!Directory.Exists(Path.Combine(Application.dataPath, TAPJOY_DIRECTORY)))
        {
          Debug.Log("Not exist");
          AssetDatabase.CreateFolder(ASSETS_DIRECTORY, TAPJOY_DIRECTORY);
        }
        string assetPath = String.Format("{0}Settings.asset", TAPJOY_SETTINGS_DIRECTORY);
        AssetDatabase.CreateAsset(settings, assetPath);
        settings = (AssetDatabase.LoadAssetAtPath(assetPath, typeof(TapjoySettings)) as TapjoySettings);
      } catch (UnityException) {
        Debug.LogError("Failed to create the Tapjoy Settings asset.");
      }
      return settings;
    }

    public static TapjoySettings Load() {
#if DEBUG
      Debug.Log("TapjoySettings.Load()");
#endif
      //Search for Tapjoy Settings.asset folders in order. Returns GUID's
      string[] results = AssetDatabase.FindAssets("Settings", new string[] { TAPJOY_SETTINGS_DIRECTORY, ASSETS_DIRECTORY });
      TapjoySettings settings = null;
      foreach (string guid in results)
      {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        Debug.Log("Using Tapjoy Path: " + path);

        //Check if asset is Tapjoy Settings.asset
        settings = (AssetDatabase.LoadAssetAtPath(path, typeof(TapjoySettings)) as TapjoySettings);
        if (settings != null)
        {
          Debug.Log("Using Tapjoy Settings: " + path);
          break;
        }
      }

      if (settings == null) {
        settings = CreateEmptySettings();
      }
      return settings;
    }

    public static void Save(TapjoySettings settings) {
      #if DEBUG
      Debug.Log("TapjoySettingsAsset.Save()");
      #endif
      EditorUtility.SetDirty(settings);
      AssetDatabase.SaveAssets();
      settings.Dirty = false;
    }
  }
}
