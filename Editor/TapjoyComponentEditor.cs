using System;
using UnityEngine;
using UnityEditor;
using TapjoyUnity;
using UnityEditor.SceneManagement;
using TapjoyUnity.Internal;

namespace TapjoyEditor {

  [CustomEditor(typeof(TapjoyComponent))]
  [CanEditMultipleObjects]
  public class TapjoyComponentEditor : Editor {
    private const string GAME_OBJECT_NAME = "TapjoyUnity";
    private const string INIT_CLASS_NAME = "TapjoyUnityInit";
    private const string INIT_SCRIPT_NAME = "TapjoyUnity.Internal.TapjoyUnityInit";
    private const string TAPJOY_DEFAULT_DIRECTORY = "Assets/TapjoySDK/";
    private const string ASSETS_DIRECTORY = "Assets";


    [MenuItem("GameObject/Create Other/Tapjoy/TapjoyUnity", true)]
    public static bool CanCreateTapjoyGameObject() {
      return ShouldFixTapjoyGameObject();
    }

    [MenuItem("GameObject/Create Other/Tapjoy/TapjoyUnity", false)]
    public static void DoCreateTapjoyGameObject() {
      FixTapjoyGameObject();
    }

    private static Type FindInitScript() {
      //Search for Init Script in Tapjoy Default & Assets dirctories. Returns GUIDS
      string[] results = AssetDatabase.FindAssets(INIT_CLASS_NAME, new string[] { TAPJOY_DEFAULT_DIRECTORY, ASSETS_DIRECTORY });

      MonoScript initScript = null;
      foreach (string guid in results)
      {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        //Check if asset is MonoScript class
        initScript = (AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript);
        if (initScript != null)
        {
          Debug.Log("Using Init Script: " + path);
          return initScript.GetClass();
        }
      }
      return null;
    }

    private static bool ShouldFixTapjoyGameObject() {
      return CheckTapjoyGameObject() != "";
    }

    private static string CheckTapjoyGameObject() {
      GameObject tapjoy = GameObject.Find(GAME_OBJECT_NAME);
      if (tapjoy == null) {
        // No Tapjoy GameObject
        return "No TapjoyUnity GameObject in this scene.";
      }

      TapjoyComponent tapjoyComponent = tapjoy.GetComponent<TapjoyComponent>();
      if (tapjoyComponent == null) {
        // The Tapjoy GameObject has no TapjoyComponent
        return "The TapjoyUnity GameObject has missing components.";
      }
      if (tapjoyComponent.settings == null) {
        // The TapjoyComponent has no TapjoySettings
        return "The TapjoyUnity GameObject has missing properties.";
      }

      if (tapjoy.GetComponent(INIT_SCRIPT_NAME) == null) {
        return "The TapjoyUnity GameObject has missing components.";
      }
      return "";
    }

    private static void FixTapjoyGameObject() {
      bool dirty = false;

      GameObject tapjoy = GameObject.Find(GAME_OBJECT_NAME);
      if (tapjoy == null) {
        tapjoy = new GameObject(GAME_OBJECT_NAME);
        dirty = true;
      }

      TapjoyComponent tapjoyComponent = tapjoy.GetComponent<TapjoyComponent>();
      if (tapjoyComponent == null) {
        tapjoyComponent = tapjoy.AddComponent<TapjoyComponent>();
        dirty = true;
      }
      if (tapjoyComponent.settings == null) {
        tapjoyComponent.settings = TapjoySettingsEditor.Load();
        dirty = true;
      }

      if (tapjoy.GetComponent(INIT_SCRIPT_NAME) == null) {
        Type type = FindInitScript();
        if (type != null) {
          tapjoy.AddComponent(type);
          dirty = true;
        }
      }

      if (dirty) {
        SetDirty(tapjoy);
      }
    }


    private static void SetDirty(GameObject tapjoy) {
      try {
        SetDirtyInternal(tapjoy);
        #if DEBUG
      } catch (Exception e) {
        Debug.Log(e);
        #else
      } catch (Exception) {
        #endif
      }
    }

    private static void SetDirtyInternal(GameObject tapjoy) {
      EditorSceneManager.MarkSceneDirty(tapjoy.scene);
    }

    private static bool ShouldFixScene() {
      return CheckScene() != "";
    }

    internal static string CheckScene() {
      string status = CheckTapjoyGameObject();
      if (status != "") {
        return status;
      }

      foreach (Component component in FindObjectsOfType(typeof(TapjoyComponent)) as Component[]) {
        if (component.gameObject.name != GAME_OBJECT_NAME) {
          // TapjoyComponent in other game objects
          return "The TapjoyComponent component is used in other GameObjects";
        }
      }
      return "";
    }

    internal static void FixScene() {
      FixTapjoyGameObject();

      foreach (Component component in FindObjectsOfType(typeof(TapjoyComponent)) as Component[]) {
        if (component.gameObject.name != GAME_OBJECT_NAME) {
          // TapjoyComponent in other game objects
          Destroy(component);
        }
      }
    }

    internal static MessageType CheckScene(bool fix, out string msg) {
      if (!fix) {
        msg = CheckScene();
        return msg == "" ? MessageType.None : MessageType.Warning;
      } else {
        FixScene();
        msg = "";
        return MessageType.None;
      }
    }

    void OnEnable() {
      #if DEBUG
      Debug.Log("TapjoyComponentEditor.OnEnable()");
      #endif
    }

    void OnDisable() {
      #if DEBUG
      Debug.Log("TapjoyComponentEditor.OnDisable()");
      #endif
    }

    public override void OnInspectorGUI() {
    }
  }
}
