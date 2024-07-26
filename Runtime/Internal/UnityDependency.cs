using System;
using TapjoyUnity.Internal.UnityCompat;

namespace TapjoyUnity.Internal {

  /// <summary>
  /// To prevent the Unity API used in TapjoyUnity from being stripped in app's build,
  /// and get access to new Unity API in runtime.
  /// </summary>
  public static class UnityDependency {

    public static void OnActiveSceneChanged(SceneCompat oldScene, SceneCompat newScene) {
      if (SceneManagerCompat.activeSceneChanged != null) {
        SceneManagerCompat.activeSceneChanged(oldScene, newScene);
      }
    }

    public static void OnSceneLoaded(SceneCompat scene, int loadMode) {
      if (SceneManagerCompat.sceneLoaded != null) {
        SceneManagerCompat.sceneLoaded(scene, loadMode);
      }
    }

    public static void OnSceneUnloaded(SceneCompat scene) {
      if (SceneManagerCompat.sceneUnloaded != null) {
        SceneManagerCompat.sceneUnloaded(scene);
      }
    }

    public static TapjoyFunc<int> sceneCount;

    public static TapjoyFunc<SceneCompat> GetActiveScene;

    public static TapjoyFunc<int, SceneCompat> GetSceneAt;

    public static TapjoyFunc<object, string> ToJson;
  }
}
