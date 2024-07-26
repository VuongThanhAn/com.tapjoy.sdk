using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace TapjoyEditor {

  internal static class Utils {

    public static bool IsSupportedBuildTarget(BuildTarget buildTarget) {
      switch (buildTarget) {
      case BuildTarget.Android:
      case BuildTarget.iOS:
        return true;
      default:
        return false;
      }
    }

    #region UnityVersion
    static Version unityVersion;

    internal static Version UnityVersion {
      get {
        if (unityVersion == null) {
          try {
            unityVersion = new Version(Regex.Match(Application.unityVersion, "^[0-9]+(\\.[0-9]+){1,3}").Value);
          } catch (Exception e) {
            #if DEBUG
            Debug.LogException(e);
            #endif
            unityVersion = new Version();
          }
        }
        return unityVersion;
      }
    }
    #endregion
  }
}
