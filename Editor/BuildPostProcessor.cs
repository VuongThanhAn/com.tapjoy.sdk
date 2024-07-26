using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

using TapjoyUnity.Internal;

namespace TapjoyEditor {

  public class BuildPostProcessor {
    [PostProcessBuild(900)]
    public static void OnPostProcessBuild(BuildTarget target, string path) {
      if (target.Equals(BuildTarget.iOS)) {
        if (PlayerSettings.strippingLevel != StrippingLevel.Disabled) {
          string msg;
          MessageType msgType = LinkXml.Check(false, true, out msg);

          if (msgType == MessageType.Error) {
            UnityEngine.Debug.LogError(msg);
          } else if (msgType == MessageType.Warning) {
            UnityEngine.Debug.LogWarning(msg);
          } else if (msgType != MessageType.None) {
            UnityEngine.Debug.Log(msg);
          }
        }
        EnableBitcode(TapjoySettingsEditor.Load().IosSettings.BitcodeEnabled, target, path);
      }
    }

     private static void EnableBitcode(bool enableBitcode, BuildTarget buildTarget, string path) {
       string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

       var project = new PBXProject();
       project.ReadFromFile(projectPath);

       string appTarget = GetTargetGuid(project, "GetUnityMainTargetGuid", "Unity-iPhone");
       string frameworkTarget = GetTargetGuid(project, "GetUnityFrameworkTargetGuid", "UnityFramework");
       if (appTarget != null) {
         project.SetBuildProperty(appTarget, "ENABLE_BITCODE", enableBitcode ? "YES" : "NO");
       }
       if (frameworkTarget != null) {
         project.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", enableBitcode ? "YES" : "NO");
       }

       project.WriteToFile(projectPath);
       UnityEngine.Debug.Log("Post Process: Bitcode " + (enableBitcode ? "enabled." : "disabled."));
     }
        private static string GetTargetGuid(PBXProject project, string method, string fallbackTarget){
            string guid = null;

            var targetGuidMethod = project.GetType().GetMethod(method);
            // Fallback for Unity Editor versions that use old style of getting targets.
            if (targetGuidMethod != null) {
                guid = (string)targetGuidMethod.Invoke(project, null);
            } else {
                guid = project.TargetGuidByName(fallbackTarget);
            }
            return guid;
        }
    }
}
