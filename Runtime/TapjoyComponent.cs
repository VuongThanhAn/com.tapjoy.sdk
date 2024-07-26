using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace TapjoyUnity.Internal {

  public class TapjoyComponent : MonoBehaviour {

    private const string GAME_OBJECT_NAME = "TapjoyUnity";

    public static TapjoyComponent FindInstance() {
      return FindObjectOfType(typeof(TapjoyComponent)) as TapjoyComponent;
    }

    private const string DISABLE_ADVERTISING_ID_CHECK = "TJC_OPTION_DISABLE_ADVERTISING_ID_CHECK";

    private static bool applicationPaused = true;
    private static bool isConnecting = false;
    private static bool triedConnecting = false;
    private static PlatformSettings app;
    private static GameObject singletonGameObject;

    Dictionary<string, System.Object> lastConnectFlags;

    private static Queue events = Queue.Synchronized(new Queue());

    private enum InternalEventType {
      RemovePlacement,
      RemoveActionRequest
    }

    private struct InternalEvent {
      public InternalEventType type;
      public string data;

      public InternalEvent(InternalEventType _type, string _data) {
        type = _type;
        data = _data;
      }
    }

    #if !DEBUG
    [HideInInspector]
    #endif
    public TapjoySettings settings;

    void Awake() {
      #if DEBUG
      Debug.Log("TapjoyComponent.Awake()");
      #endif

      switch (Application.platform) {
      case RuntimePlatform.Android:
        {
          app = settings.AndroidSettings;
          break;
        }
      case RuntimePlatform.IPhonePlayer:
        {
          app = settings.IosSettings;
          break;
        }
      default:
        {
          Debug.LogWarning("Tapjoy doesn't support " + Application.platform.ToString());
          app = new PlatformSettings();
          break;
        }
      }

      ApiBinding.OnInstanceSet = OnApiBindingSet;

      EnsureSingleton();
    }

    void OnApiBindingSet() {
      #if DEBUG
      Debug.Log("TapjoyComponent.OnApiBindingSet()");
      #endif

      ApiBinding.Instance.SetDebugEnabled(settings.DebugEnabled);
    }

    private void EnsureSingleton() {
      if (gameObject == null) {
        #if DEBUG
        Debug.LogWarning("Could not find a game object of Tapjoy");
        #endif
      } else if (singletonGameObject == null) {
        singletonGameObject = gameObject;
        DontDestroyOnLoad(singletonGameObject);
      } else if (singletonGameObject != gameObject) {
        Destroy(gameObject);
      }
    }

    internal void Reconnect() {
      if (isConnecting) {
        return;
      }
      if (Tapjoy.IsConnected) {
        return;
      }

      if (!triedConnecting) {
        Debug.LogWarning("Must first call connect with an SDK key.");
        return;
      }

      // Try re-connecting with last parameters
      ApiBinding.Instance.Connect(app.SdkKey, lastConnectFlags);
    }

    internal bool ConnectManually() {
      return ConnectManually(app.SdkKey, null);
    }

    internal bool ConnectManually(string sdkKey) {
      return ConnectManually(sdkKey, null);
    }

    internal bool ConnectManually(string sdkKey, Dictionary<string, System.Object> flags) {
      if (isConnecting) {
        return false;
      }
      if (Tapjoy.IsConnected) {
        return false;
      }

      app.SdkKey = sdkKey;
      return ConnectInternal(flags);
    }

    private bool ConnectInternal(Dictionary<string, System.Object> connectFlags) {
      if (!app.Valid) {
        if (Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer) {
          Debug.LogWarning("Please check if you applied correct SDK key.");
        }
        return false;
      }

      Tapjoy.OnConnectSuccessInternal -= HandleOnConnectSuccess;
      Tapjoy.OnConnectSuccessInternal += HandleOnConnectSuccess;

      Tapjoy.OnConnectFailedInternal -= HandleOnConnectFailed;
      Tapjoy.OnConnectFailedInternal += HandleOnConnectFailed;

      Tapjoy.OnConnectWarningInternal -= HandleOnConnectWarning;
      Tapjoy.OnConnectWarningInternal += HandleOnConnectWarning;

      if (connectFlags == null) {
        connectFlags = new Dictionary<string, System.Object>();
      }

      if (!connectFlags.ContainsKey(DISABLE_ADVERTISING_ID_CHECK) && app.DisableAdvertisingId) {
        connectFlags.Add(DISABLE_ADVERTISING_ID_CHECK, app.DisableAdvertisingId);
      }

      lastConnectFlags = connectFlags;
      isConnecting = true;
      triedConnecting = true;
      ApiBinding.Instance.Connect(app.SdkKey, connectFlags);
      return true;
    }

    void OnDestroy() {
      #if DEBUG
      Debug.Log("TapjoyComponent: OnDestroy");
      #endif
      Tapjoy.OnConnectSuccessInternal -= HandleOnConnectSuccess;
      Tapjoy.OnConnectFailedInternal -= HandleOnConnectFailed;
      Tapjoy.OnConnectWarningInternal -= HandleOnConnectWarning;
      isConnecting = false;
    }

    void HandleOnConnectSuccess() {
      #if DEBUG
      Debug.Log("TapjoyComponent.HandleOnConnectSuccess");
      #endif
      isConnecting = false;
    }

    void HandleOnConnectFailed (int code, string message)
    {
#if DEBUG
      Debug.Log ("TapjoyComponent.HandleOnConnectFailed. code: " + code + " " + message);
#endif
      isConnecting = false;
    }

    void HandleOnConnectWarning(int code, string message) {
      #if DEBUG
      Debug.Log("TapjoyComponent.HandleOnConnectWarning. code: " + code + " " + message);
      #endif
    }

    void Start() {
      if (settings.AutoConnectEnabled) {
        ConnectManually();
      }

      if (applicationPaused) {
        applicationPaused = false;
      }
    }

    void Update() {
      if (events.Count > 0) {
        try {
          // Processing only one event is enough
          InternalEvent ev = (InternalEvent) events.Dequeue();
          if (ev.type == InternalEventType.RemovePlacement) {
            ApiBinding.Instance.RemovePlacement(ev.data);
          } else if (ev.type == InternalEventType.RemoveActionRequest) {
            ApiBinding.Instance.RemoveActionRequest(ev.data);
          }
        } catch (System.InvalidOperationException) {
          // no events
        }
      }
    }

    void OnApplicationPause(bool paused) {
      if (app == null) { // Workaround for exception in the editor play mode of Unity 3.5.7.
        return;
      }

      if (applicationPaused != paused) {
        applicationPaused = paused;
        ForegroundRealtimeClock.OnApplicationPause(paused);
      }
    }

    void OnApplicationQuit() {
      if (!applicationPaused) {
        applicationPaused = true;
      }
    }

    // Native Listener Callbacks for Connect (Native -> C#)
    void OnNativeConnectCallback(string commaDelimitedMessage) {
      Tapjoy.DispatchConnectEvent(commaDelimitedMessage);
    }

    // Native Listener Callbacks for Set User ID (Native -> C#)
    void OnNativeSetUserIDCallback(string commaDelimitedMessage) {
      Tapjoy.DispatchSetUserIDEvent(commaDelimitedMessage);
    }

    // Native Listener Callbacks for Currency calls (Native -> C#)
    void OnNativeCurrencyCallback(string commaDelimitedMessage) {
      Tapjoy.DispatchCurrencyEvent(commaDelimitedMessage);
    }

    //  Native Listener Callbacks for TJPlacement (Native -> C#)
    void OnNativePlacementCallback(string commaDelimitedMessage) {
      TJPlacement.DispatchPlacementEvent(commaDelimitedMessage);
    }

    //  Native Listener Callbacks for TJOfferwallDiscover (Native -> C#)
    void OnNativeOfferwallDiscoverCallback(string commaDelimitedMessage)
    {
        TJOfferwallDiscover.DispatchOfferwallDiscoverEvent(commaDelimitedMessage);
    }
    
    // NOTE: calling Android methods directly from destructor causes crash.
    // Instead, pass it here to remove.
    public static void RemovePlacement(string placementID) {
      events.Enqueue(new InternalEvent(InternalEventType.RemovePlacement, placementID));
    }

    public static void RemoveActionRequest(string requestID) {
      events.Enqueue(new InternalEvent(InternalEventType.RemoveActionRequest, requestID));
    }

  }
}
