using System;
using UnityEngine;

namespace TapjoyUnity.Internal {

  static class ForegroundRealtimeClock {

    static float realtimeSpentWhilePaused = 0;

    static float realtimePaused = 0;

    /// <summary>
    /// Should be called in MonoBehaviour.OnApplicationPause.
    /// </summary>
    internal static void OnApplicationPause(bool paused) {
      if (paused) {
        realtimePaused = Time.realtimeSinceStartup;
      } else {
        realtimeSpentWhilePaused += Time.realtimeSinceStartup - realtimePaused;
        realtimePaused = 0;
      }
    }

    internal static float Realtime {
      get {
        return Time.realtimeSinceStartup - realtimeSpentWhilePaused;
      }
    }
  }
}
