namespace TapjoyUnity.Internal.UnityCompat {

  /// <see cref="UnityEngine.SceneManagement.Scene"/>
  public struct SceneCompat {

    internal static readonly SceneCompat NONE = new SceneCompat();

    public SceneCompat(object scene, bool valid, int buildIndex, string name, string path) {
      _hashCode = scene.GetHashCode();
      _valid = valid;
      _buildIndex = buildIndex;
      _name = name;
      _path = path;
    }

    readonly int _hashCode;

    public override int GetHashCode() {
      return _hashCode;
    }

    readonly bool _valid;

    #if TEST
    public
    #else
    internal
    #endif
    bool IsValid() {
      return _valid;
    }

    readonly int? _buildIndex;

    #if TEST
    public
    #else
    internal
    #endif
    int buildIndex {
      get {
        return _buildIndex ?? -1;
      }
    }

    readonly string _name;

    #if TEST
    public
    #else
    internal
    #endif
    string name {
      get {
        return _name;
      }
    }

    readonly string _path;

    #if TEST
    public
    #else
    internal
    #endif
    string path {
      get {
        return _path;
      }
    }

    #if DEBUG
    public override string ToString() {
      return string.Format("SceneCompat(hashCode={0} valid={1} buildIndex={2} name={3} path={4})",
        GetHashCode(), IsValid(), buildIndex, name, path);
    }
    #endif
  }

}
