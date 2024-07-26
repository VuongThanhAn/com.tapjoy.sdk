using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;
using TapjoyUnity.Internal;

namespace TapjoyEditor {

  internal static class LinkXml {

    private const string ASSETS_LINK_XML = "Assets/TapjoySDK/link.xml";

    public static string Path() {
      return ASSETS_LINK_XML;
    }

    public static MessageType Check(bool fix, bool warningAsError, out string msg) {
      msg = "";

      if (File.Exists(LinkXml.Path())) {
        XmlDocument linkXml = new XmlDocument();
        try {
          linkXml.Load(ASSETS_LINK_XML);
        } catch (Exception) {
          return MessageType.None;
        }

        if (linkXml.DocumentElement.Name == "linker") {
          XmlElement assemblyElement =
            linkXml.SelectSingleNode("//assembly[@fullname=\"Tapjoy\"]") as XmlElement;
          if (assemblyElement != null) {
            if (fix) {
              linkXml.DocumentElement.RemoveChild(assemblyElement);
              linkXml.Save(ASSETS_LINK_XML);
              AssetDatabase.Refresh();
            } else {
              msg = ASSETS_LINK_XML + " contains an unnecessary element";
              return warningAsError ? MessageType.Error : MessageType.Warning;
            }
          }
        }
      }

      return MessageType.None;
    }
  }
}
