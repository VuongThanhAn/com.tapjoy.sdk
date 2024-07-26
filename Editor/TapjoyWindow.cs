using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;
using TapjoyUnity.Internal;

namespace TapjoyEditor {

  public class TapjoyWindow : EditorWindow {

    [MenuItem("Window/Tapjoy")]
    static void Init() {
      EditorWindow window = EditorWindow.GetWindow(typeof(TapjoyWindow), false, "Tapjoy");
      window.minSize = new Vector2(250f, 300f);
    }

    private static Texture2D tapjoyIcon;
    private static Texture2D questionMarkIcon;

    private static Texture2D rectTexture;
    private static GUIStyle rectStyle;
    private static GUIStyle headerLabelStyle;
    private static GUIStyle foldoutStyle;
    private static GUIStyle pressedButton;

    private TapjoySettings settings;
    private AppStoreSettings appStoreSettings;
    private bool obsoleteAssetsChecked = false;
    private bool obsoleteAssetsExist = false;

    private enum Tab {
      Android,
      Ios,
    };

    private Tab selectedTab = Tab.Android;

    private Vector2 scrollPosition;

    void OnEnable() {
      #if DEBUG
      Debug.Log("TapjoyWindow.OnEnable()");
      #endif

      settings = TapjoySettingsEditor.Load();
      appStoreSettings = new AppStoreSettings();
      tapjoyIcon = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TapjoySDK/Editor/Images/tapjoy-icon.png", typeof(Texture2D));
      questionMarkIcon = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TapjoySDK/Editor/Images/question-mark.png", typeof(Texture2D));

      switch (EditorUserBuildSettings.activeBuildTarget) {
      case BuildTarget.Android:
        selectedTab = Tab.Android;
        break;
      case BuildTarget.iOS:
        selectedTab = Tab.Ios;
        break;
      }
      Repaint();
    }

    void OnDisable() {
      #if DEBUG
      Debug.Log("TapjoyWindow.OnDisable()");
      #endif
      settings = null;
    }

    void OnSelectionChange() {
      #if DEBUG
      Debug.Log("TapjoyWindow.OnSelectionChange()");
      #endif
    }

    void OnHierarchyChange() {
      #if DEBUG
      Debug.Log("TapjoyWindow.OnProjectChange()");
      #endif
      Repaint();
    }

    void OnProjectChange() {
      #if DEBUG
      Debug.Log("TapjoyWindow.OnProjectChange()");
      #endif
      Repaint();
    }

    void OnInspectorUpdate() {
      #if DEBUG
      Debug.Log("TapjoyWindow.OnInspectorUpdate()");
      #endif
      Repaint();
    }

    private void OnGUI() {
      // Check if we need to make resources
      if (rectTexture == null) {
        rectTexture = new Texture2D(1, 1);
        rectTexture.SetPixel(0, 0, new Color(0.87f, 0.87f, 0.87f));
        rectTexture.Apply();

        rectStyle = new GUIStyle();
        rectStyle.normal.background = rectTexture;
      }

      if (pressedButton == null) {
        pressedButton = new GUIStyle("button");
      }

      if (headerLabelStyle == null) {
        headerLabelStyle = EditorStyles.boldLabel;
      }

      if (foldoutStyle == null) {
        foldoutStyle = EditorStyles.foldout;
        foldoutStyle.fontStyle = FontStyle.Bold;
        foldoutStyle.focused.background = foldoutStyle.normal.background;
      }

      GUI.SetNextControlName("ClearFocus");

      DrawHeader();

      DrawSDKSettingsUI();

      DrawTabs();

      DrawTabContent();

      if (settings.Dirty) {
        TapjoySettingsEditor.Save(settings);
      }
      if (appStoreSettings.Dirty) {
        AndroidManifest.CheckAppStore(appStoreSettings);
        appStoreSettings.Dirty = false;
      }
    }

    private void DrawHeader() {
      GUI.Box(new Rect(0, 0, position.width, 40), "", rectStyle);

      if (tapjoyIcon != null) {
        GUI.Label(new Rect(5, 5, tapjoyIcon.width, tapjoyIcon.height), tapjoyIcon);
      }

      GUIStyle headerFontStyle = new GUIStyle();
      headerFontStyle.fontSize = 14;
      GUI.Label(new Rect(45, 10, 200, 30), "Tapjoy SDK - " + TapjoyUnity.Tapjoy.Version, headerFontStyle);

      if (questionMarkIcon != null) {
        if (GUI.Button(new Rect(position.width - 35, 5, 30, 30), questionMarkIcon)) {
          Application.OpenURL("http://dev.tapjoy.com/sdk-integration/unity/getting-started-guide-publishers-unity/");
        }
      }

      GUILayout.Space(45);
    }

    private void DrawSDKSettingsUI() {
      GUILayout.Label("SDK Settings", EditorStyles.boldLabel);

      GUIStyle headerStyle = new GUIStyle();
      headerStyle.padding = new RectOffset(10, 10, 0, 0);

      GUILayout.BeginVertical(headerStyle);

      settings.DebugEnabled = EditorGUILayout.Toggle("Debug Mode", settings.DebugEnabled);

      settings.AutoConnectEnabled = EditorGUILayout.Toggle("Auto-Connect", settings.AutoConnectEnabled);
      if (!settings.AutoConnectEnabled) {
        EditorGUILayout.HelpBox("Must manually call Tapjoy.Connect() during runtime.", MessageType.Info);
      }

      GUILayout.EndVertical();

      int failures = 0;
      string msg = "";
      // Run checks for status
      Checker(TapjoyComponentEditor.CheckScene(false, out msg), msg, ref failures, false);
      Checker(CheckLinkXml(false, out msg), msg, ref failures, false);
      Checker(CheckObsoleteAssets(false, out msg), msg, ref failures, false);
      Checker(Unity4Support.CheckPlatformSpecificAssets(false, out msg), msg, ref failures, false);
      if (Utils.UnityVersion.Major >= 5) {
        Checker(Unity5Support.CheckGooglePlayServicesPlatformSettings(false, out msg), msg, ref failures, false);
      }

      string gameObjectStatus = failures > 0 ? "ERROR" : "OK";

      GUILayout.Label("Tapjoy GameObject Status [" + gameObjectStatus + "]", headerLabelStyle);
      // If there were errors show them in UI
      if (failures > 0) {
        failures = 0;
        msg = "";
        // Now draw the UI for Errors
        if (Checker(TapjoyComponentEditor.CheckScene(false, out msg), msg, ref failures)) {
          TapjoyComponentEditor.CheckScene(true, out msg);
        }
        if (Checker(CheckLinkXml(false, out msg), msg, ref failures)) {
          CheckLinkXml(true, out msg);
        }
        if (Checker(CheckObsoleteAssets(false, out msg), msg, ref failures)) {
          CheckObsoleteAssets(true, out msg);
        }
        if (Checker(Unity4Support.CheckPlatformSpecificAssets(false, out msg), msg, ref failures)) {
          Unity4Support.CheckPlatformSpecificAssets(true, out msg);
        }
        if (Utils.UnityVersion.Major >= 5) {
          if (Checker(Unity5Support.CheckGooglePlayServicesPlatformSettings(false, out msg), msg, ref failures)) {
            Unity5Support.CheckGooglePlayServicesPlatformSettings(true, out msg);
          }
        }
      }
    }

    private void DrawTabs() {
      GUIStyle verticalStyle = new GUIStyle();
      verticalStyle.padding = new RectOffset(5, 5, 0, 0);
      GUILayout.BeginVertical(verticalStyle);
      GUILayout.BeginHorizontal();

      pressedButton.normal.background = pressedButton.active.background;

      if (selectedTab == Tab.Android) {
        if (GUILayout.Button(" Android ", pressedButton)) {
          selectedTab = Tab.Android;
        }
      } else {
        if (GUILayout.Button(" Android ")) {
          selectedTab = Tab.Android;
          GUIUtility.keyboardControl = 0;
        }
      }

      if (selectedTab == Tab.Ios) {
        if (GUILayout.Button("   iOS   ", pressedButton)) {
          selectedTab = Tab.Ios;
        }
      } else {
        if (GUILayout.Button("   iOS   ")) {
          selectedTab = Tab.Ios;
          GUIUtility.keyboardControl = 0;
        }
      }

      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    private void DrawTabContent() {
      scrollPosition = GUILayout.BeginScrollView(scrollPosition);

      switch (selectedTab) {
      case Tab.Android:
        DrawAndroidContent();
        break;
      case Tab.Ios:
        DrawIosContent();
        break;
      default:
        break;
      }

      GUILayout.EndScrollView();
    }

    private void DrawAndroidContent() {
      GUILayout.Label("Android Settings", EditorStyles.boldLabel);
      GUIStyle verticalStyle = new GUIStyle();
      verticalStyle.padding = new RectOffset(10, 10, 5, 5);
      GUILayout.BeginVertical(verticalStyle);

      // SDK key
      if (settings.AutoConnectEnabled) {
        settings.AndroidSettings.SdkKey = EditorGUILayout.TextField("SDK Key", settings.AndroidSettings.SdkKey);
        if (settings.AndroidSettings.SdkKey == "") {
          EditorGUILayout.HelpBox("SDK Key Missing for this platform", MessageType.Info);
        }
      } else {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("SDK Key", "");
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.HelpBox("Auto-Connect disabled: SDK Key must be set dynamically via Tapjoy.Connect(sdkKey)", MessageType.Warning);
      }

      // Store
      int selectedAppStore = appStoreSettings.GetAppStoreIndex();
      string[] appStores = appStoreSettings.GetAppStores();
      string isAndroidManifestAvailableMsg;

      if (AndroidManifest.MessageType.None == AndroidManifest.IsAndroidManifestAvailable(out isAndroidManifestAvailableMsg)) {
        selectedAppStore = EditorGUILayout.Popup("Store", selectedAppStore, appStores);
        appStoreSettings.SetAppStoreIndex(selectedAppStore);

        if (appStoreSettings.IsCustomAppStoreIndex(selectedAppStore)) {
          appStoreSettings.AppStore = EditorGUILayout.TextField(" ", appStoreSettings.AppStore);
        }
      } else {
        GUILayout.Label("Store", EditorStyles.label);
      }

      // Disable Advertising Id
      settings.AndroidSettings.DisableAdvertisingId = EditorGUILayout.Toggle("Disable Advertising Id", settings.AndroidSettings.DisableAdvertisingId);

      // Integration
      EditorGUILayout.Separator();
      GUILayout.EndVertical();
    }

    private void CheckGCMReceiver(string msg, ref int failures, bool renderUI = true) {
      if (Checker(AndroidManifest.CheckGCMReceiver(false, out msg), msg, ref failures, renderUI)) {
        AndroidManifest.CheckGCMReceiver(true, out msg);
      }
      if (failures == 0 && renderUI) {
        EditorGUILayout.HelpBox("OK", MessageType.None);
      }
    }

    private void DrawIosContent() {
      GUILayout.Label("iOS Settings", EditorStyles.boldLabel);
      GUIStyle verticalStyle = new GUIStyle();
      verticalStyle.padding = new RectOffset(10, 10, 5, 5);
      GUILayout.BeginVertical(verticalStyle);

      if (settings.AutoConnectEnabled) {
        settings.IosSettings.SdkKey = EditorGUILayout.TextField("SDK Key", settings.IosSettings.SdkKey);
        if (settings.IosSettings.SdkKey == "") {
          EditorGUILayout.HelpBox("SDK Key Missing for this platform", MessageType.Info);
        }
      } else {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("SDK Key", "");
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.HelpBox("Auto-Connect disabled: SDK Key must be set dynamically via Tapjoy.Connect(sdkKey)", MessageType.Warning);
      }

      bool bitcodeToggled = EditorGUILayout.Toggle(new GUIContent("Enable Bitcode", "As of iOS 16 and Xcode 14 bitcode is being deprecated. Leaving it enabled may result in build issues. Refer to Xcode 14 release notes for more information.\n\nhttps://developer.apple.com/documentation/Xcode-Release-Notes/xcode-14-release-notes"), settings.IosSettings.BitcodeEnabled);
      settings.IosSettings.BitcodeEnabled = bitcodeToggled;
      EditorGUILayout.Separator();

      GUILayout.EndVertical();
    }


    private bool Checker(MessageType msgType, string msg, ref int failures, bool renderUI = true) {
      return Checker((AndroidManifest.MessageType) msgType, msg, ref failures, renderUI);
    }

    private bool Checker(AndroidManifest.MessageType msgType, string msg, ref int failures, bool renderUI = true) {
      bool clicked = false;
      switch (msgType) {
      case AndroidManifest.MessageType.Warning:
      case AndroidManifest.MessageType.WarningCantFix:
        failures++;
        if (renderUI) {
          EditorGUILayout.BeginHorizontal();
          EditorGUILayout.HelpBox(msg, MessageType.Warning);

          if (msgType == AndroidManifest.MessageType.Warning) {
            if (GUILayout.Button("Fix", GUILayout.Width(60), GUILayout.Height(40))) {
              clicked = true;
            }
          }
          EditorGUILayout.EndHorizontal();
        }
        break;
      case AndroidManifest.MessageType.Error:
        failures++;
        if (renderUI) {
          EditorGUILayout.HelpBox(msg, MessageType.Error);
        }
        break;
      }
      return clicked;
    }

    private void GUIEnable(bool condition) {
      GUI.enabled = condition;
    }

    private void GUIEnable() {
      GUI.enabled = true;
    }

    private static string[] OBSOLETE_FILES = new string[] {
      "Assets/Plugins/5Rocks/FiveRocks.cs",
      "Assets/Plugins/5Rocks/JSONObject.cs",

      // since 2.2.0
      "Assets/Plugins/iOS/FiveRocksC.h",
      "Assets/Plugins/iOS/FiveRocksUnitySupport.h",

      // since 2.2.3
      "Assets/Plugins/5Rocks/FiveRocksBinding.cs",

      // since Tapjoy SDK 11.6, around Unity version 11.3.4
      "Assets/Editor/AssetsBundler.cs",
      "Assets/Editor/PostprocessBuildPlayer",
      "Assets/Editor/PostprocessBuildPlayer_Tapjoy",
      "Assets/Editor/TapjoyXcodeUpdatePostBuild.pyc",
    };

    private MessageType CheckObsoleteAssets(bool fix, out string msg) {
      if (fix) {
        obsoleteAssetsChecked = false;
        obsoleteAssetsExist = false;
        bool changed = false;
        AssetDatabase.Refresh();
        foreach (string obsolete in OBSOLETE_FILES) {
          if (File.Exists(obsolete)) {
            changed = AssetDatabase.MoveAssetToTrash(obsolete) || changed;
          }
        }
        if (changed) {
          AssetDatabase.Refresh();
        }
      } else if (!obsoleteAssetsChecked) {
        obsoleteAssetsChecked = true;
        obsoleteAssetsExist = false;
        AssetDatabase.Refresh();
        foreach (string obsolete in OBSOLETE_FILES) {
          if (File.Exists(obsolete)) {
            obsoleteAssetsExist = true;
            msg = "Obsolete SDK asset found";
            return MessageType.Warning;
          }
        }
      } else if (obsoleteAssetsExist) {
        msg = "Obsolete SDK asset found";
        return MessageType.Warning;
      }
      msg = "";
      return MessageType.None;
    }

    private MessageType lastResultCheckedLinkXml = MessageType.None;
    private string lastMessageCheckedLinkXml = "";
    private DateTime lastFileTimeCheckedLinkXml = DateTime.MinValue;

    private MessageType CheckLinkXml(bool fix, out string msg) {
      if (!File.Exists(LinkXml.Path())) {
        lastResultCheckedLinkXml = MessageType.None;
        lastMessageCheckedLinkXml = "";
        lastFileTimeCheckedLinkXml = DateTime.MinValue;
        msg = "";
        return MessageType.None;
      }

      if (!fix) {
        DateTime fileTime = File.GetLastWriteTime(LinkXml.Path());
        if (lastFileTimeCheckedLinkXml == fileTime) {
          msg = lastMessageCheckedLinkXml;
          return lastResultCheckedLinkXml;
        }
        lastFileTimeCheckedLinkXml = fileTime;
        lastResultCheckedLinkXml = LinkXml.Check(fix, false, out msg);
        lastMessageCheckedLinkXml = msg;
      } else {
        lastFileTimeCheckedLinkXml = DateTime.MinValue;
        lastResultCheckedLinkXml = LinkXml.Check(fix, false, out msg);
        lastMessageCheckedLinkXml = msg;
      }
      return lastResultCheckedLinkXml;
    }
  }
}
