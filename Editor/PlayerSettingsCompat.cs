using System;
using System.Reflection;
using UnityEditor;

namespace TapjoyEditor {

  internal static class PlayerSettingsCompat {

    #region applicationIdentifier
    static PropertyInfo applicationIdentifierPropertyInfo;

    /// <summary>
    /// Provides access to
    /// <see cref="UnityEditor.PlayerSettings.applicationIdentifier"/> available since Unity 5.6.0
    /// or <see cref="UnityEditor.PlayerSettings.bundleIdentifier"/>.
    /// </summary>
    internal static string applicationIdentifier {
      get {
        if (applicationIdentifierPropertyInfo == null) {
          applicationIdentifierPropertyInfo = typeof(PlayerSettings).GetProperty("applicationIdentifier");
          if (applicationIdentifierPropertyInfo == null) {
            applicationIdentifierPropertyInfo = typeof(PlayerSettings).GetProperty("bundleIdentifier");
          }
        }
        return (string) applicationIdentifierPropertyInfo.GetValue(null, null);
      }
    }
    #endregion

  }
}
