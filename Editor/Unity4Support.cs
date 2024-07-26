using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace TapjoyEditor {

  static class Unity4Support {

    internal static void PreBuild() {
      if (Utils.UnityVersion.Major >= 5) {
        return;
      }
      string msg;
      CheckPlatformSpecificAssets(true, out msg);
    }

    internal static MessageType CheckPlatformSpecificAssets(bool fix, out string msg) {
      msg = "";
      if (Utils.UnityVersion.Major >= 5) {
        return MessageType.None;
      }
      if (!CheckPlatformSpecificAsset("47962600855c14a76b5687f6a6ae2e5e", "Tapjoy.Android.dll", BuildTarget.Android, fix)
          || !CheckPlatformSpecificAsset("aed174e0c3e644f358fc096cb36ed2b8", "Tapjoy.iOS.dll", BuildTarget.iOS, fix)) {
        msg = "Some platform-specific assets should be rearranged";
        return MessageType.Warning;
      }
      return MessageType.None;
    }

    static bool CheckPlatformSpecificAsset(string guid, string name, BuildTarget buildTarget, bool fix) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      if (EditorUserBuildSettings.activeBuildTarget == buildTarget) {
        if (path == "") {
          Debug.LogError(string.Format("Could not find {0}", name));
          return false;
        } else if (Path.GetFileName(path) != name) {
          if (fix) {
            if (AssetDatabase.MoveAsset(path, Path.Combine(Path.GetDirectoryName(path), name)) != "") {
              Debug.LogError(string.Format("Could not include {0} in build", name));
              return false;
            }
          } else {
            return false;
          }
        }
      } else {
        if (path != "" && Path.GetFileName(path) == name) {
          if (fix) {
            if (AssetDatabase.MoveAsset(path, path + "_") != "") {
              Debug.LogError(string.Format("Could not exclude {0} from build", name));
              return false;
            }
          } else {
            return false;
          }
        }
      }
      return true;
    }
  }
}
