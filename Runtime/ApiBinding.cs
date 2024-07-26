using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace TapjoyUnity.Internal {

  public abstract class ApiBinding {

    protected const string DEFAULT_EVENT_VALUE_NAME = "value";
    protected const string VERSION_NAME = Tapjoy.VERSION_NAME;

    protected const string CONNECT_FLAG_DICTIONARY_NAME = "connectFlags";

    private static ApiBinding instance = null;

    public static ApiBinding Instance {
      get {
        if (instance == null) {
          instance = new ApiBindingNone();
        }
        return instance;
      }
    }

    protected static void SetInstance(ApiBinding value) {
      if (instance == null || instance is ApiBindingNone) {
        #if DEBUG
        Debug.Log("Installed " + value);
        #endif
        instance = value;
        if (onInstanceSetHandler != null) {
          onInstanceSetHandler.Invoke();
        }
      }
    }

    #if TEST
    public static void SetInstanceForTesting(ApiBinding instance) {
      ApiBinding.instance = instance;
    }
    #endif

    internal delegate void OnInstanceSetHandler();

    static OnInstanceSetHandler onInstanceSetHandler;

    internal static OnInstanceSetHandler OnInstanceSet {
      set {
        onInstanceSetHandler = value;
        if (value != null && !(instance is ApiBindingNone)) {
          value.Invoke();
        }
      }
    }

    private string name;

    protected ApiBinding(string name) {
      this.name = name;
    }

    internal string Name {
      get {
        return name;
      }
    }

    // Connect
    public abstract void Connect(string sdkKey, Dictionary<string, System.Object> flag);

    // Config
    public abstract string GetSDKVersion();

    public abstract void SetDebugEnabled(bool enabled);

    public abstract void ActivateUnitySupport();

    // Tapjoy
    public abstract void GetPrivacyPolicy();

    public abstract void SetSubjectToGDPR (TJStatus subject);

    public abstract int GetSubjectToGDPR ();

    public abstract void SetUserConsent (TJStatus consent);

    public abstract int GetUserConsent ();

    public abstract void SetBelowConsentAge (TJStatus belowConsentAge);

    public abstract int GetBelowConsentAge ();

    public abstract void SetUSPrivacy(string privacyConsent);

    public abstract string GetUSPrivacy ();

    public abstract void OptOutAdvertisingID(bool optOut);
    
    public abstract void GetCurrencyBalance();

    public abstract void SpendCurrency(int points);

    public abstract void AwardCurrency(int points);

    public abstract string GetSupportURL();

    public abstract string GetSupportURL(string currencyID);

    public abstract void ShowDefaultEarnedCurrencyAlert();

    public abstract void ActionComplete(string actionID);

    // Tapjoy Placement
    public abstract void CreatePlacement(string placementGuid, string placementName);

    public abstract void DismissPlacementContent();

    public abstract void RequestPlacementContent(string placementGuid);

    public abstract void ShowPlacementContent(string placementGuid);

    public abstract void ActionRequestCompleted(string requestId);

    public abstract void ActionRequestCancelled(string requestId);

    public abstract bool IsPlacementContentReady(string placementGuid);

    public abstract bool IsPlacementContentAvailable(string placementGuid);

    public abstract void RemovePlacement(string placementGuid);

    public abstract void SetEntryPoint(string placementGuid, TJEntryPoint entryPoint);

    public abstract void SetCurrencyBalance(string placementGuid, string currencyId, int balance);

    public abstract int GetCurrencyBalance(string placementGuid, string currencyId);

    public abstract void SetRequiredAmount (string placementGuid, string currencyId, int amount);

    public abstract int GetRequiredAmount (string placementGuid, string currencyId);

    // Offerwall Discover
    public abstract void RequestOfferwallDiscover(string placementName, float height);

    public abstract void RequestOfferwallDiscover(string placementName, float left, float top, float width, float height);

    public abstract void ShowOfferwallDiscover();

    public abstract void DestroyOfferwallDiscover();

    public abstract void RemoveActionRequest(string requestId);

    // 5Rocks Cohorts
    public abstract void SetUserID(string userId);

    public abstract string GetUserID();

    // Currency Callback Param
    public abstract void SetCustomParameter(string customParam);

    public abstract string GetCustomParameter();

    public abstract void SetUserLevel(int userLevel);

    public abstract int GetUserLevel();

    public abstract void SetMaxLevel(int maxUserLevel);

    public abstract int GetMaxLevel();

    public abstract void SetUserSegment(TJSegment userSegment);

    public abstract int GetUserSegment();

    public abstract double GetScreenScale();

    // User Tag
    public abstract void ClearUserTags();

    public abstract List<string> GetUserTags();

    public abstract void AddUserTag(string tag);

    public abstract void RemoveUserTag(string tag);

    public abstract void TrackPurchase(string currencyCode, double price);
  }
}
