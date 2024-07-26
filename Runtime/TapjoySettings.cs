using System;
using System.Collections.Generic;
using UnityEngine;

namespace TapjoyUnity.Internal {

  public class TapjoySettings : ScriptableObject {

    #if !DEBUG
    [HideInInspector]
    #endif
    [SerializeField]
    private PlatformSettings androidSettings = new PlatformSettings();

    #if !DEBUG
    [HideInInspector]
    #endif
    [SerializeField]
    private IosPlatformSettings iosSettings = new IosPlatformSettings();

    #if !DEBUG
    [HideInInspector]
    #endif
    [SerializeField]
    private bool autoConnectEnabled = true;

    #if !DEBUG
    [HideInInspector]
    #endif
    [SerializeField]
    private bool debugEnabled = false;

    private bool dirty = false;

    public TapjoySettings() {
      #if DEBUG
      Debug.Log("new TapjoySettings()");
      #endif
    }

    public PlatformSettings AndroidSettings {
      get {
        return androidSettings;
      }
    }

    public IosPlatformSettings IosSettings {
      get {
        return iosSettings;
      }
    }

    public bool AutoConnectEnabled {
      get {
        return autoConnectEnabled;
      }
      set {
        if (autoConnectEnabled != value) {
          autoConnectEnabled = value;
          dirty = true;
        }
      }
    }

    public bool DebugEnabled {
      get {
        return debugEnabled;
      }
      set {
        if (debugEnabled != value) {
          debugEnabled = value;
          dirty = true;
        }
      }
    }

    public bool Dirty {
      get {
        return dirty || androidSettings.Dirty || iosSettings.Dirty;
      }
      set {
        dirty = value;
        androidSettings.Dirty = value;
        iosSettings.Dirty = value;
      }
    }
  }
}
