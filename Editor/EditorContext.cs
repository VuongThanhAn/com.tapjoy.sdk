using System;
using UnityEngine;
using UnityEditor;
using TapjoyUnity;

namespace TapjoyEditor {

  internal static class EditorContext {

    private static BuildTarget buildTarget;
    private static bool isSupportedBuildTarget;
    private static string platform = string.Empty;

    public static void CheckBuildTarget() {
      if (buildTarget != EditorUserBuildSettings.activeBuildTarget) {
        #if DEBUG
        Debug.Log("EditorContext.OnBuildTargetChanged: to=" + EditorUserBuildSettings.activeBuildTarget);
        #endif
        buildTarget = EditorUserBuildSettings.activeBuildTarget;
        isSupportedBuildTarget = true;
        switch (buildTarget) {
        case BuildTarget.Android:
          platform = "Android";
          break;
        case BuildTarget.iOS:
          platform = "iOS";
          break;
        default:
          isSupportedBuildTarget = false;
          platform = buildTarget + " (Not Supported)";
          break;
        }
      }
    }

    public static bool IsSupportedBuildTarget {
      get {
        return isSupportedBuildTarget;
      }
    }

    public static string Platform {
      get {
        return platform;
      }
    }
  }
}
