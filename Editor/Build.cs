using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TapjoyEditor {

  /**
   * @brief Provides utility methods for batch build pipeline.
   */
  public sealed class Build {

    /**
     * @brief Performs preprocessing required before building.
     */
    public static void PreBuild() {
      Unity4Support.PreBuild();
    }

    /**
     * @brief Builds the active build target with PreBuild.
     */
    public static void BuildPlayer() {
      BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
      string output = EditorUserBuildSettings.GetBuildLocation(target);
      string[] scenes = GetScenesInBuild();
      BuildOptions options = BuildOptions.None;

      PreBuild();
      BuildPipeline.BuildPlayer(scenes, output, target, options);
    }

    static string[] GetScenesInBuild() {
      List<string> names = new List<string>();
      foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
        if (scene != null) {
          if (scene.enabled) {
            names.Add(scene.path);
          }
        }
      }
      return names.ToArray();
    }
  }
}
