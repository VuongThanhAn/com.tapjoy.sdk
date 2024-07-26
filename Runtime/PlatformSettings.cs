using System;
using UnityEngine;

namespace TapjoyUnity.Internal {

  [Serializable]
  public class PlatformSettings {
    [SerializeField]
    private string sdkKey = string.Empty;
    [SerializeField]
    private bool disableAdvertisingId = false;

    protected bool dirty = false;


    public PlatformSettings() {
    }

    public string SdkKey {
      get {
        return sdkKey;
      }
      set {
        if (sdkKey != value) {
          sdkKey = value;
          dirty = true;
        }
      }
    }

    public bool DisableAdvertisingId {
      get {
        return disableAdvertisingId;
      }
      set {
        if (disableAdvertisingId != value) {
          disableAdvertisingId = value;
          dirty = true;
        }
      }
    }
    
    public bool Valid {
      get {
        return sdkKey != "";
      }
    }

    public bool Dirty {
      get {
        return dirty;
      }
      set {
        dirty = value;
      }
    }
  }
  
  [Serializable]
  public class IosPlatformSettings : PlatformSettings {
      [SerializeField]
      private bool bitcodeEnabled = false;
      
      public bool BitcodeEnabled {
      get {
        return bitcodeEnabled;
      }
      set {
        if (bitcodeEnabled != value) {
          bitcodeEnabled = value;
          dirty = true;
        }
      }
    }
  }
}
