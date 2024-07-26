using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TapjoyUnity.Internal {

  internal static class Utils {

    #region UnityVersion
    internal readonly static Version VERSION_5_4 = new Version(5, 4);

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
