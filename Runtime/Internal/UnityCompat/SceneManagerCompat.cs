namespace TapjoyUnity.Internal.UnityCompat {

  /// <see cref="UnityEngine.SceneManagement.SceneManager"/>
  internal class SceneManagerCompat {

    /// <see cref="UnityEngine.SceneManagement.SceneManager.activeSceneChanged"/>
    /// which is available since Unity 5.4.0.
    internal static TapjoyAction<SceneCompat, SceneCompat> activeSceneChanged;

    /// <see cref="UnityEngine.SceneManagement.SceneManager.sceneLoaded"/>
    /// which is available since Unity 5.4.0.
    internal static TapjoyAction<SceneCompat, int> sceneLoaded;

    /// <see cref="UnityEngine.SceneManagement.SceneManager.sceneUnloaded"/>
    /// which is available since Unity 5.4.0.
    internal static TapjoyAction<SceneCompat> sceneUnloaded;

    /// <see cref="UnityEngine.SceneManagement.SceneManager.sceneCount"/>
    /// which is available since Unity 5.4.0.
    internal static int sceneCount {
      get {
        if (UnityDependency.sceneCount != null) {
          return UnityDependency.sceneCount();
        }
        return 0;
      }
    }

    /// <see cref="UnityEngine.SceneManagement.SceneManager.GetActiveScene"/>
    /// which is available since Unity 5.4.0.
    internal static SceneCompat GetActiveScene() {
      if (UnityDependency.GetActiveScene != null) {
        return UnityDependency.GetActiveScene();
      }
      return SceneCompat.NONE;
    }

    /// <see cref="UnityEngine.SceneManagement.SceneManager.GetSceneAt"/>
    /// which is available since Unity 5.4.0.
    internal static SceneCompat GetSceneAt(int index) {
      if (UnityDependency.GetSceneAt != null) {
        return UnityDependency.GetSceneAt(index);
      }
      return SceneCompat.NONE;
    }
  }
}
