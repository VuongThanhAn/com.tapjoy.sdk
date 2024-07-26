using System;
using UnityEngine;
using UnityEditor;

namespace TapjoyEditor {

  static class Unity5Support {
    internal readonly static Version VERSION_5_4 = new Version(5, 4);

    internal static MessageType CheckGooglePlayServicesPlatformSettings(bool fix, out string msg) {
      if (Utils.UnityVersion >= VERSION_5_4) {
        PluginImporter pluginImporter = PluginImporter.GetAtPath("Assets/Plugins/Android/google-play-services_lib") as PluginImporter;
        if (pluginImporter != null) {
          if (!fix) {
            if (pluginImporter.GetCompatibleWithAnyPlatform()
                || !pluginImporter.GetCompatibleWithPlatform(BuildTarget.Android)
                || pluginImporter.GetCompatibleWithPlatform(BuildTarget.iOS)) {
              msg = "google-play-services_lib is not set to be only compatible with Android";
              return MessageType.Warning;
            } else {
              msg = "";
              return MessageType.None;
            }
          } else {
            pluginImporter.SetCompatibleWithAnyPlatform(false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.iOS, false);
            msg = "";
            return MessageType.None;
          }
        }
      }

      msg = "";
      return MessageType.None;
    }
  }
}
