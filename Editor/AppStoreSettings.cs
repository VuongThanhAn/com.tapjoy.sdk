using System;
using System.IO;

using UnityEngine;
using UnityEditor;

using TapjoyUnity.Internal;

namespace TapjoyEditor {

  #if DEBUG
  public class AppStoreSettings
  #else
  internal class AppStoreSettings
  #endif
  {
    /// null : Not Set, "" : Custom
    private string appStore = null;
    private int lastAppStoreIndex;
    //private const string PREFS_APPSTORE = "com.tapjoy.appstore";
    private string[] appStores = { "(not set)", "Google", "Amazon", "Tstore", "Custom" };

    private bool dirty = false;

    public AppStoreSettings() {
      try {
        appStore = AndroidManifest.LoadAppStore();
      } catch {
      }
      if (string.IsNullOrEmpty(appStore)) {
        appStore = null;
      }
      lastAppStoreIndex = GetAppStoreIndex();
    }

    public string [] GetAppStores() {
      return appStores;
    }

    public int GetAppStoreIndex() {
      if (appStore == null) {
        return 0;
      }
      for (int i = 1; i < appStores.Length - 1; ++i) {
        if (appStore.Equals(appStores[i])) {
          return i;
        }
      }
      return appStores.Length - 1;
    }

    public bool IsCustomAppStoreIndex(int index) {
      if (0 <= index && index < appStores.Length - 1) {
        return false;
      }
      return true;
    }

    public void SetAppStoreIndex(int index) {
      if (lastAppStoreIndex != index) {
        if (IsCustomAppStoreIndex(index)) {
          AppStore = string.Empty;
        } else if (index == 0) {
          AppStore = null;
        } else {
          AppStore = appStores[index];
        }
        lastAppStoreIndex = index;
      }
    }

    public string AppStore {
      get {
        return appStore;
      }
      set {
        if (value != null) {
          value = value.Trim();
        }

        if (appStore != value) {
          appStore = value;
          dirty = true;
        }
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
}
