using System;
using System.Collections.Generic;
using TapjoyUnity;

namespace TapjoyUnity
{
  public enum TJEntryPoint
  {
    UNKNOWN,
    OTHER,
    MAIN_MENU,
    HUD,
    EXIT,
    FAIL,
    COMPLETE,
    INBOX,
    INIT,
    STORE
  }
}
public static class EntryPointExtensions
{
  public static readonly Dictionary<TJEntryPoint, string> entryPointValues = new Dictionary<TJEntryPoint, string>
  {
        { TJEntryPoint.UNKNOWN, "unknown" },
        { TJEntryPoint.OTHER, "other" },
        { TJEntryPoint.MAIN_MENU, "main_menu" },
        { TJEntryPoint.HUD, "hud" },
        { TJEntryPoint.EXIT, "exit" },
        { TJEntryPoint.FAIL, "fail" },
        { TJEntryPoint.COMPLETE, "complete" },
        { TJEntryPoint.INBOX, "inbox" },
        { TJEntryPoint.INIT, "initialisation" },
        { TJEntryPoint.STORE, "store" }
  };

  public static string GetValue(this TJEntryPoint entryPoint)
  {
    return entryPointValues[entryPoint];
  }
}