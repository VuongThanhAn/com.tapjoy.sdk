using System;
using System.Xml;
using System.IO;
using System.Collections;

using UnityEngine;
using UnityEditor;

using TapjoyUnity.Internal;

namespace TapjoyEditor {

  #if DEBUG
  public static class AndroidManifest
  #else
  internal static class AndroidManifest
  #endif
  {
    private const string MANIFEST_PATH = "Assets/Plugins/Android/AndroidManifest.xml";
    private const string NS_ANDROID = "http://schemas.android.com/apk/res/android";

    private static readonly string[] GCM_ACTIONS = {
      "com.google.android.c2dm.intent.REGISTRATION",
      "com.google.android.c2dm.intent.RECEIVE",
      "com.google.android.gcm.intent.RETRY"
    };
    private const string TJ_GCMReceiver = "com.tapjoy.GCMReceiver";
    private const string TJ_TapjoyReceiver = "com.tapjoy.TapjoyReceiver";
    private const string PERMISSION_SEND = "com.google.android.c2dm.permission.SEND";


    private static DateTime[] lastFileTimeChecked =
      { DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue };
    private static string[] lastMessageChecked =
      { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
    private static MessageType[] lastResultChecked =
      { MessageType.None, MessageType.None, MessageType.None, MessageType.None, MessageType.None, MessageType.None };

    public enum MessageType {
      None = UnityEditor.MessageType.None,
      Info = UnityEditor.MessageType.Info,
      Warning = UnityEditor.MessageType.Warning,
      Error = UnityEditor.MessageType.Error,
      WarningCantFix,
    };

    enum CheckAndFixMenu {
      GCMReceiver = 0,
    };

    public static MessageType IsAndroidManifestAvailable(out string msg) {
      if (File.Exists(MANIFEST_PATH)) {
        msg = string.Empty;
        return MessageType.None;
      }
      msg = MANIFEST_PATH + " is not found";
      return MessageType.WarningCantFix;
    }

    public static MessageType CheckGCMReceiver(bool fix, out string msg) {
      return CheckAndFix(fix, out msg, CheckAndFixMenu.GCMReceiver);
    }

    private static MessageType CheckAndFix(bool fix, out string msg, CheckAndFixMenu menu) {
      int menuIndex = (int) menu;
      if (!fix) {
        if (!File.Exists(MANIFEST_PATH)) {
          lastFileTimeChecked[menuIndex] = DateTime.MinValue;
          msg = MANIFEST_PATH + " is not found";
          return MessageType.WarningCantFix;
        }
        DateTime fileTime = File.GetLastWriteTime(MANIFEST_PATH);
        if (lastFileTimeChecked[menuIndex] == fileTime) {
          msg = lastMessageChecked[menuIndex];
          return lastResultChecked[menuIndex];
        }
        lastFileTimeChecked[menuIndex] = fileTime;
        lastResultChecked[menuIndex] = CheckAndFix_(fix, out msg, menu);
        lastMessageChecked[menuIndex] = msg;
      } else {
        lastFileTimeChecked[menuIndex] = DateTime.MinValue;
        lastResultChecked[menuIndex] = CheckAndFix_(fix, out msg, menu);
        lastMessageChecked[menuIndex] = msg;
      }
      return lastResultChecked[menuIndex];
    }

    private static MessageType CheckAndFix_(bool fix, out string msg, CheckAndFixMenu menu) {
      try {
        msg = string.Empty;

        XmlDocument xml = new XmlDocument();
        xml.Load(MANIFEST_PATH);
        XmlNamespaceManager xmlNsManager = new XmlNamespaceManager(xml.NameTable);
        xmlNsManager.AddNamespace("android", NS_ANDROID);

        XmlNode rootNode = xml.SelectSingleNode("/manifest");

        bool canFix = true;

        if (rootNode == null) {
          canFix = false;
          msg = MANIFEST_PATH + " is malformed";
        } else if (menu == CheckAndFixMenu.GCMReceiver) {
          canFix = CanFixGCMReceiver(xml, rootNode, xmlNsManager, out msg);
        }
        // Permissions are always fixable if we have rootNode

        if (canFix) {
          bool ok = false;
          bool modified = false;
          string settingName = string.Empty;

          if (menu == CheckAndFixMenu.GCMReceiver) {
            settingName = "GCMReceiver";
            modified = CheckGCMReceiver(xml, rootNode, xmlNsManager, fix, out ok);
          }

          if (modified) {
            SaveXml(xml);
          }

          if (ok) {
            msg = string.Empty;
            return MessageType.None;
          } else {
            if (string.IsNullOrEmpty(msg)) {
              msg = settingName + " setting in " + MANIFEST_PATH + " is invalid";
            }
            return MessageType.Warning;
          }
        } else {
          return MessageType.Error;
        }
      } catch (FileNotFoundException) {
        msg = MANIFEST_PATH + " is not found";
        return MessageType.WarningCantFix;
      } catch (DirectoryNotFoundException) {
        msg = MANIFEST_PATH + " is not found";
        return MessageType.WarningCantFix;
      } catch (XmlException) {
        msg = MANIFEST_PATH + " is malformed";
        return MessageType.Error;
      }
    }

    public static string LoadAppStore() {
      string result = string.Empty;
      XmlDocument xml = new XmlDocument();

      xml.Load(MANIFEST_PATH);
      if (xml.DocumentElement.Name == "manifest") {
        XmlNamespaceManager xmlNsManager = new XmlNamespaceManager(xml.NameTable);
        xmlNsManager.AddNamespace("android", NS_ANDROID);

        const string ATTR_NAME = "com.tapjoy.appstore";

        XmlNode nodeAppStore = xml.SelectSingleNode(
                                 "/manifest/application/meta-data[@android:name='" + ATTR_NAME + "']", xmlNsManager);

        if (nodeAppStore != null) {
          XmlAttribute value = (XmlAttribute) nodeAppStore.Attributes.GetNamedItem("value", NS_ANDROID);
          if (value != null) {
            result = value.Value;
          }
        }
      }
      return result;
    }

    public static bool CheckAppStore(AppStoreSettings appStoreSettings) {
      bool modified = false;

      XmlDocument xml = new XmlDocument();
      try {
        xml.Load(MANIFEST_PATH);
        if (xml.DocumentElement.Name == "manifest") {
          XmlNamespaceManager xmlNsManager = new XmlNamespaceManager(xml.NameTable);
          xmlNsManager.AddNamespace("android", NS_ANDROID);

          XmlNode appNode = xml.SelectSingleNode("/manifest/application");
          if (appNode == null) {
            Debug.LogError(MANIFEST_PATH + " is malformed.");
          } else {
            // <meta-data android:name="com.tapjoy.appstore" android:value="Google"/>
            modified |= CheckAppStore_(xml, appNode, xmlNsManager, appStoreSettings.AppStore);

            if (modified) {
              SaveXml(xml);
            }
          }
        } else {
          Debug.LogError(MANIFEST_PATH + " is malformed. Root element is not <manifest>");
        }
      } catch (XmlException) {
        Debug.LogError(MANIFEST_PATH + " is malformed.");
      } catch {
        // FileNotFoundException, DirectoryNotFoundException
      }
      return modified;
    }

    private static bool CheckNetPermissions(XmlDocument xml, XmlNode rootNode, XmlNamespaceManager xmlNsManager, bool fix, out bool ok) {
      bool modified = false;
      bool ok_ = false;

      const string PERMISSION_INTERNET = "android.permission.INTERNET";

      XmlNode insertAfter = GetLastElement(rootNode, "/manifest/uses-permission");

      modified |= CheckEachPermission(
        xml, rootNode, ref insertAfter, xmlNsManager, "uses-permission", PERMISSION_INTERNET, fix, out ok_);

      ok = ok_;

      return modified;
    }

    private static bool CheckPushPermissions(XmlDocument xml, XmlNode rootNode, XmlNamespaceManager xmlNsManager, bool fix, out bool ok) {
      bool modified = false;

      const string PERMISSION_RECEIVE = "com.google.android.c2dm.permission.RECEIVE";
      const string PERMISSION_GET_ACCOUNT = "android.permission.GET_ACCOUNTS";
      string PERMISSION_C2D_MESSAGE = PlayerSettingsCompat.applicationIdentifier + ".permission.C2D_MESSAGE";

      XmlNode insertAfter = GetLastElement(rootNode, "/manifest/uses-permission");

      ok = true;
      bool ok_ = false;

      modified |= CheckEachPermission(
        xml, rootNode, ref insertAfter, xmlNsManager, "uses-permission", PERMISSION_RECEIVE, fix, out ok_);
      ok &= ok_;

      modified |= CheckEachPermission(
        xml, rootNode, ref insertAfter, xmlNsManager, "uses-permission", PERMISSION_GET_ACCOUNT, fix, out ok_);
      ok &= ok_;

      modified |= CheckEachPermission(
        xml, rootNode, ref insertAfter, xmlNsManager, "permission", PERMISSION_C2D_MESSAGE, fix, out ok_);
      ok &= ok_;

      modified |= CheckEachPermission(
        xml, rootNode, ref insertAfter, xmlNsManager, "uses-permission", PERMISSION_C2D_MESSAGE, fix, out ok_);
      ok &= ok_;

      return modified;
    }

    private static bool CheckEachPermission(XmlDocument xml, XmlNode rootNode, ref XmlNode insertAfter,
                                            XmlNamespaceManager xmlNsManager, string elemName, string permission, bool fix, out bool ok) {
      XmlNode node = rootNode.SelectSingleNode(
                       "/manifest/" + elemName + "[@android:name='" + permission + "']",
                       xmlNsManager);

      ok = true;
      if (node == null) {
        ok = false;
        if (fix) {
          XmlElement elem = xml.CreateElement(elemName);
          elem.SetAttribute("name", NS_ANDROID, permission);

          // NOTE: Assume that permission elem always have protectionLevel=signature
          if (elemName.Equals("permission")) {
            elem.SetAttribute("protectionLevel", NS_ANDROID, "signature");
          }

          if (insertAfter == null) {
            rootNode.AppendChild(elem);
          } else {
            rootNode.InsertAfter(elem, insertAfter);
          }
          Debug.Log("Added " + permission + " into " + MANIFEST_PATH);
          insertAfter = elem;
          return true;
        }
      }
      return false;
    }

    private static XmlNode GetManifestApplicationNode(XmlNode rootNode, out string msg) {
      XmlNode appNode = rootNode.SelectSingleNode("/manifest/application");

      if (appNode == null) {
        msg = "<application> is not found in " + MANIFEST_PATH;
        return null;
      }
      msg = string.Empty;
      return appNode;
    }

    private static bool CanFixGCMReceiver(XmlDocument xml, XmlNode rootNode, XmlNamespaceManager xmlNsManager, out string msg) {
      return null != GetManifestApplicationNode(rootNode, out msg);
    }

    private static bool CheckGCMReceiver(XmlDocument xml, XmlNode rootNode, XmlNamespaceManager xmlNsManager, bool fix, out bool ok) {
      bool modified = false;
      bool ok_ = false;
      ok = true;

      // permissions and receivers
      modified |= CheckNetPermissions(xml, rootNode, xmlNsManager, fix, out ok_);
      ok &= ok_;

      modified |= CheckPushPermissions(xml, rootNode, xmlNsManager, fix, out ok_);
      ok &= ok_;

      modified |= CheckPushReceivers(xml, rootNode, xmlNsManager, fix, out ok_);
      ok &= ok_;

      return modified;
    }

    private static bool CheckPushReceivers(XmlDocument xml, XmlNode rootNode, XmlNamespaceManager xmlNsManager, bool fix, out bool ok) {
      bool modified = false;

      XmlNode appNode = rootNode.SelectSingleNode("/manifest/application");
      if (appNode == null) {
        ok = false;
        return false;
      }

      // Get attributes of receiver, intent-filter
      // Use this for replacing old GCMReceiver
      XmlAttributeCollection receiverAttrs = null;
      XmlAttributeCollection intentFilterAttrs = null;


      XmlNode elemTemp = appNode.SelectSingleNode(
                           "receiver[@android:name='" + TJ_GCMReceiver + "']", xmlNsManager);
      if (elemTemp != null) {
        receiverAttrs = elemTemp.Attributes;
      }

      elemTemp = appNode.SelectSingleNode(
        "receiver[@android:name='" + TJ_GCMReceiver + "']/intent-filter", xmlNsManager);

      if (elemTemp != null) {
        intentFilterAttrs = elemTemp.Attributes;
      }

      // Check Other GCMReceiver
      XmlNode nodeGCMReceiver = appNode.SelectSingleNode(
                                  "receiver[@android:permission='" + PERMISSION_SEND + "']",
                                  xmlNsManager);

      if (nodeGCMReceiver != null) {
        XmlAttribute name = (XmlAttribute) nodeGCMReceiver.Attributes.GetNamedItem("name", NS_ANDROID);
        if (name != null && name.Value.Equals(TJ_GCMReceiver)) {
          // 5Rocks GCM Receiver available. Check it
          ok = true;
          for (int i = 0; i < GCM_ACTIONS.Length; ++i) {
            XmlNode elemAction = nodeGCMReceiver.SelectSingleNode(
                                   "intent-filter/action[@android:name='" + GCM_ACTIONS[i] + "']", xmlNsManager);
            if (elemAction == null) {
              ok = false;
              break;
            }
          }

          if (ok) {
            XmlNode elemCat = nodeGCMReceiver.SelectSingleNode(
                                "intent-filter/category[@android:name='" +
                                PlayerSettingsCompat.applicationIdentifier + "']", xmlNsManager);

            if (elemCat == null) {
              ok = false;
            }
          }

          // If not ok, replace it
          if (!ok && fix) {
            XmlNode nextNode = nodeGCMReceiver.NextSibling;
            RemoveAllElements(
              "/manifest/application/receiver[@android:name='" + TJ_GCMReceiver + "']",
              appNode, xmlNsManager);
            if (nextNode == null) {
              appNode.AppendChild(CreateGCMReceiver(xml, receiverAttrs, intentFilterAttrs));
            } else {
              appNode.InsertBefore(CreateGCMReceiver(xml, receiverAttrs, intentFilterAttrs), nextNode);
            }
            modified = true;
            //Debug.Log(MANIFEST_PATH + ": " + TJ_GCMReceiver + " is fixed");
          }
        } else {
          ok = false;
          // Other GCM Receiver first.
          // Remove any old GCMReceiver and prepend new GCMReceiver element
          if (fix) {
            RemoveAllElements(
              "/manifest/application/receiver[@android:name='" + TJ_GCMReceiver + "']",
              appNode,
              xmlNsManager);
            //Debug.Log(MANIFEST_PATH + ": Other GCM Receiver first. Let " + TJ_GCMReceiver + " be first");
            appNode.InsertBefore(CreateGCMReceiver(xml, receiverAttrs, intentFilterAttrs), nodeGCMReceiver);
            modified = true;
          }
        }
      } else {
        ok = false;
        // No GCM Receiver. Append GCMReceiver element
        //Debug.Log(MANIFEST_PATH + ": " + TJ_GCMReceiver + " is added");
        if (fix) {
          appNode.AppendChild(CreateGCMReceiver(xml, null, null));
          modified = true;
        }
      }

      // Check TapjoyReceiver
      XmlNode node2 = appNode.SelectSingleNode(
                        "/manifest/application/receiver[@android:name='" + TJ_TapjoyReceiver + "']",
                        xmlNsManager);

      if (node2 == null) {
        ok = false;
        if (fix) {
          appNode.AppendChild(CreateTapjoyReceiver(xml));
          modified = true;
        }
        //Debug.Log(MANIFEST_PATH + ": " + TJ_TapjoyReceiver + " is added");
      }

      return modified;
    }

    private static bool CheckAppStore_(XmlDocument xml, XmlNode appNode, XmlNamespaceManager xmlNsManager, string appStore) {
      const string ATTR_NAME = "com.tapjoy.appstore";
      bool modified = false;
      int removed = RemoveAllElements(
                      "/manifest/application/meta-data[@android:name='" + ATTR_NAME + "']", appNode, xmlNsManager);
      if (removed > 0) {
        modified = true;
      }

      XmlElement meta = xml.CreateElement("meta-data");
      meta.SetAttribute("name", NS_ANDROID, ATTR_NAME);
      if (!string.IsNullOrEmpty(appStore)) {
        meta.SetAttribute("value", NS_ANDROID, appStore);
        appNode.AppendChild(meta);
        modified = true;
      }

      return modified;
    }

    private static XmlElement CreateGCMReceiver(XmlDocument xml, XmlAttributeCollection receiverAttrs, XmlAttributeCollection intentFilterAttrs) {
      XmlElement elemRoot = xml.CreateElement("receiver");
      elemRoot.SetAttribute("name", NS_ANDROID, TJ_GCMReceiver);
      elemRoot.SetAttribute("permission", NS_ANDROID, PERMISSION_SEND);

      if (receiverAttrs != null) {
        IEnumerator i = receiverAttrs.GetEnumerator();
        while (i.MoveNext()) {
          XmlAttribute attr = (XmlAttribute) ((XmlAttribute) i.Current).Clone();
          elemRoot.SetAttributeNode(attr);
        }
      }

      XmlElement elemIntent = xml.CreateElement("intent-filter");
      if (intentFilterAttrs != null) {
        IEnumerator i = intentFilterAttrs.GetEnumerator();
        while (i.MoveNext()) {
          XmlAttribute attr = (XmlAttribute) ((XmlAttribute) i.Current).Clone();
          elemIntent.SetAttributeNode(attr);
        }
      }

      for (int i = 0; i < GCM_ACTIONS.Length; ++i) {
        XmlElement elemAction = xml.CreateElement("action");
        elemAction.SetAttribute("name", NS_ANDROID, GCM_ACTIONS[i]);
        elemIntent.AppendChild(elemAction);
      }
      XmlElement elemCat = xml.CreateElement("category");

      elemCat.SetAttribute("name", NS_ANDROID, PlayerSettingsCompat.applicationIdentifier);
      elemIntent.AppendChild(elemCat);
      elemRoot.AppendChild(elemIntent);

      return elemRoot;
    }

    private static XmlElement CreateTapjoyReceiver(XmlDocument xml) {
      XmlElement elem = xml.CreateElement("receiver");
      elem.SetAttribute("name", NS_ANDROID, TJ_TapjoyReceiver);
      return elem;
    }

    private static XmlElement CreateActivity(XmlDocument xml, string name, bool hardwareAcceleratedRequired, string theme) {
      XmlElement elem = xml.CreateElement("activity");
      elem.SetAttribute("name", NS_ANDROID, name);
      elem.SetAttribute("configChanges", NS_ANDROID, "orientation|keyboardHidden|screenSize");
      if (theme != null) {
        elem.SetAttribute("theme", NS_ANDROID, theme);
      }
      if (hardwareAcceleratedRequired) {
        elem.SetAttribute("hardwareAccelerated", NS_ANDROID, "true");
      }

      return elem;
    }

    private static XmlNode GetLastElement(XmlNode rootNode, string xpath) {
      XmlNodeList nodes = rootNode.SelectNodes(xpath);
      XmlNode result = null;
      if (nodes != null) {
        IEnumerator ienum = nodes.GetEnumerator();
        while (ienum.MoveNext()) {
          result = (XmlNode) ienum.Current;
        }
      }
      return result;
    }

    private static int RemoveAllElements(string xpath, XmlNode node, XmlNamespaceManager xmlNsManager) {
      int count = 0;
      XmlNodeList nodeList = node.SelectNodes(xpath, xmlNsManager);

      if (nodeList == null) {
        return 0;
      }

      IEnumerator ienum = nodeList.GetEnumerator();
      bool hasNext = ienum.MoveNext();
      while (hasNext) {
        XmlNode prevNode = (XmlNode) ienum.Current;
        hasNext = ienum.MoveNext();
        prevNode.ParentNode.RemoveChild(prevNode);
        count++;
      }
      return count;
    }

    private static void SaveXml(XmlDocument xml) {
      using (TextWriter sw = new StreamWriter(MANIFEST_PATH, false, System.Text.Encoding.UTF8)) {
        xml.Save(sw);
      }
    }
  }
}
