using System;

namespace TapjoyUnity.Internal.UnityCompat {

  public class JsonUtilityCompat {

    /// <see cref="UnityEngine.JsonUtility.ToJson"/> which is available since Unity 5.4.0
    internal static string ToJson(object obj) {
      if (UnityDependency.ToJson != null) {
        return UnityDependency.ToJson(obj);
      }
      return "";
    }
  }
}
