using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;

#if UNITY_ANDROID

namespace TapjoyUnity.Internal {

  public sealed class ApiBindingAndroid : ApiBinding {

    public static void Install() {
      ApiBinding.SetInstance(new ApiBindingAndroid());
    }

    private AndroidJavaClass tapjoyJavaAPI;
    private AndroidJavaClass tjSupportClass;
    private bool canPassNullArgument;

    ApiBindingAndroid() : base("Android") {
      tapjoyJavaAPI = new AndroidJavaClass("com.tapjoy.Tapjoy");
      tjSupportClass = new AndroidJavaClass("com.tapjoy.TapjoyConnectUnity");
      canPassNullArgument = CanPassNullArgument();

      tjSupportClass.CallStatic("activate", VERSION_NAME);
    }

    private static bool CanPassNullArgument() {
      double version = 0;
      Match match = Regex.Match(Application.unityVersion, "^[0-9]+.[0-9]+");
      if (match.Success && double.TryParse(match.Groups[0].Value, out version)) {
        if (version >= 4.1) {
          return true;
        }
      }
      return false;
    }

    // Connect
    public override void Connect(string sdkKey, Dictionary<string, System.Object> flags) {
      if (sdkKey == null) {
        return;
      }
      if (flags != null) {
        // Convert C# connectFlags dictionary to Java dictionary
        foreach (KeyValuePair<string, System.Object> kvp in flags) {
          // Is the value is a dictionary itself
          if (kvp.Value.GetType().IsGenericType) {
            Dictionary<string, System.Object> dictionaryToTransfer = (Dictionary<string, System.Object>) kvp.Value;

            String dictionaryName = kvp.Key;
            // Communicate with java to build the dictionary
            transferDictionaryToJavaWithName(dictionaryToTransfer, dictionaryName);
            // Tell java to add the dictionary *dictionaryName* to dictionary "connectFlags" under the key *kvp.key*
            tjSupportClass.CallStatic("setDictionaryInDictionary", kvp.Key, dictionaryName, CONNECT_FLAG_DICTIONARY_NAME);
          } else {
            // Tell java to add the value *kvp.Value* to dictionary "connectFlags" under the key *kvp.Key*
            tjSupportClass.CallStatic("setKeyValueInDictionary", kvp.Key, kvp.Value, CONNECT_FLAG_DICTIONARY_NAME);
          }
        }
      }

      tjSupportClass.CallStatic("connect", sdkKey);
    }

    private void transferDictionaryToJavaWithName(Dictionary<string, System.Object> dictionary, String dictionaryName) {
      foreach (KeyValuePair<string, System.Object> kvp in dictionary) {
        tjSupportClass.CallStatic("setKeyValueInDictionary", kvp.Key, kvp.Value, dictionaryName);
      }
    }

    private void transferDictionaryToJavaWithName (Dictionary<string, string> dictionary, String dictionaryName)
    {
      foreach (KeyValuePair<string, string> kvp in dictionary) {
        tjSupportClass.CallStatic ("setKeyValueInDictionary", kvp.Key, kvp.Value, dictionaryName);
      }
    }

    public override void ActionComplete(string actionID) {
      tapjoyJavaAPI.CallStatic("actionComplete", actionID);
    }

    // Config
    public override string GetSDKVersion() {
      return tapjoyJavaAPI.CallStatic<string>("getVersion");
    }

    public override void SetDebugEnabled(bool enabled) {
      tapjoyJavaAPI.CallStatic("setDebugEnabled", enabled);
    }

    public override void GetPrivacyPolicy() {
      tjSupportClass.CallStatic("getPrivacyPolicy");
    }

    public override void SetSubjectToGDPR(TJStatus gdprApplicable) {
      tjSupportClass.CallStatic("setSubjectToGDPR", (int)gdprApplicable);
    }

    public override int GetSubjectToGDPR ()
    {
      return tjSupportClass.CallStatic<int> ("getSubjectToGDPR");
    }

    public override void SetUserConsent(TJStatus consent) {
      tjSupportClass.CallStatic("setUserConsent", (int)consent);
    }

    public override int GetUserConsent ()
    {
      return tjSupportClass.CallStatic<int> ("getUserConsent");
    }

    public override void SetBelowConsentAge(TJStatus belowConsentAge) {
      tjSupportClass.CallStatic("setBelowConsentAge", (int)belowConsentAge);
    }

    public override int GetBelowConsentAge ()
    {
      return tjSupportClass.CallStatic<int> ("getBelowConsentAge");
    }

    public override void SetUSPrivacy(string privacyConsent) {
      tjSupportClass.CallStatic("setUSPrivacy", privacyConsent);
    }

    public override string GetUSPrivacy ()
    {
      return tjSupportClass.CallStatic<string> ("getUSPrivacy");
    }

    public override void OptOutAdvertisingID (bool optOut)
    {
      tjSupportClass.CallStatic ("optOutAdvertisingID", optOut);
    }

    public override void ActivateUnitySupport() {
      tjSupportClass.CallStatic("activate", VERSION_NAME);
    }

    // Currency Calls (Use tjSupport to implement Java callback listeners)
    public override void AwardCurrency(int amount) {
      tjSupportClass.CallStatic("awardCurrency", amount);
    }

    public override void GetCurrencyBalance() {
      tjSupportClass.CallStatic("getCurrencyBalance");
    }

    public override void SpendCurrency(int amount) {
      tjSupportClass.CallStatic("spendCurrency", amount);
    }

    public override string GetSupportURL() {
        return tjSupportClass.CallStatic<string>("getSupportURL");
    }

    public override string GetSupportURL(string currencyID) {
        return tjSupportClass.CallStatic<string>("getSupportURL", currencyID);
    }

    public override void ShowDefaultEarnedCurrencyAlert() {
      tjSupportClass.CallStatic("showDefaultEarnedCurrencyAlert");
    }

    // Tapjoy Placement
    public override void CreatePlacement(string placementGuid, string placementName) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      placementName = MakeStringUnitySafe(placementName);

      tjSupportClass.CallStatic("createPlacement", placementGuid, placementName);
    }

    public override void DismissPlacementContent() {
      tjSupportClass.CallStatic("dismissPlacementContent");
    }

    public override void RequestPlacementContent(string placementGuid) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      tjSupportClass.CallStatic("requestPlacementContent", placementGuid);
    }

    public override void ShowPlacementContent(string placementGuid) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      tjSupportClass.CallStatic("showPlacementContent", placementGuid);
    }

    public override bool IsPlacementContentReady(string placementGuid) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      return tjSupportClass.CallStatic<bool>("isPlacementContentReady", placementGuid);
    }

    public override bool IsPlacementContentAvailable(string placementGuid) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      return tjSupportClass.CallStatic<bool>("isPlacementContentAvailable", placementGuid);
    }

    public override void SetCurrencyBalance (string placementGuid, string currencyId, int balance) {
      placementGuid = MakeStringUnitySafe (placementGuid);
      tjSupportClass.CallStatic("setCurrencyBalance", placementGuid, currencyId, balance);
    }

    public override int GetCurrencyBalance (string placementGuid, string currencyId) {
      return tjSupportClass.CallStatic<int> ("getCurrencyBalance", placementGuid, currencyId);
    }

    public override void SetRequiredAmount (string placementGuid, string currencyId, int amount) {
      placementGuid = MakeStringUnitySafe (placementGuid);
      tjSupportClass.CallStatic("setCurrencyAmountRequired", placementGuid, currencyId, amount);
    }

    public override int GetRequiredAmount (string placementGuid, string currencyId) {
      return tjSupportClass.CallStatic<int> ("getCurrencyAmountRequired", placementGuid, currencyId);
    }

    // Action Requests
    public override void ActionRequestCompleted(string requestID) {
      requestID = MakeStringUnitySafe(requestID);
      tjSupportClass.CallStatic("actionRequestCompleted", requestID);
    }

    public override void ActionRequestCancelled(string requestID) {
      requestID = MakeStringUnitySafe(requestID);
      tjSupportClass.CallStatic("actionRequestCancelled", requestID);
    }

    public override void RemovePlacement(string placementGuid) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      tjSupportClass.CallStatic("removePlacement", placementGuid);
    }

    public override void RequestOfferwallDiscover(string placementName, float height) {
        tjSupportClass.CallStatic("requestOWDiscover", placementName, height);
    }

    public override void RequestOfferwallDiscover(string placementName, float left, float top, float width, float height)
    {
      tjSupportClass.CallStatic ("requestOWDiscover", placementName, left, top, width, height);
    }

    public override void ShowOfferwallDiscover() {
        tjSupportClass.CallStatic("showOWDiscover");
    }

    public override void DestroyOfferwallDiscover() {
        tjSupportClass.CallStatic("destroyOWDiscover");
    }

    public override void RemoveActionRequest(string requestID) {
      requestID = MakeStringUnitySafe(requestID);
      tjSupportClass.CallStatic("removeActionRequest", requestID);
    }

    public override void SetEntryPoint(string placementGuid, TJEntryPoint entryPoint) {
      placementGuid = MakeStringUnitySafe(placementGuid);
      string entryPointString = MakeStringUnitySafe(entryPoint.GetValue());
      tjSupportClass.CallStatic("setEntryPoint", placementGuid, entryPointString);
    }

    // 5Rocks Cohorts
    public override void SetUserID(string userId) {
      userId = MakeStringUnitySafe(userId);
      tjSupportClass.CallStatic("setUserID", userId);
    }

    public override string GetUserID() {
      return tjSupportClass.CallStatic<string>("getUserID");
    }

    public override void SetCustomParameter(string customParam) {
      customParam = MakeStringUnitySafe(customParam);
      tapjoyJavaAPI.CallStatic("setCustomParameter", customParam);
    }

    public override string GetCustomParameter() {
      return tapjoyJavaAPI.CallStatic<string>("getCustomParameter");
    }

    public override void SetUserLevel(int userLevel) {
      tapjoyJavaAPI.CallStatic("setUserLevel", userLevel);
    }

    public override int GetUserLevel() {
      return tapjoyJavaAPI.CallStatic<int>("getUserLevel");
    }

    public override void SetMaxLevel(int maxUserLevel) {
      tapjoyJavaAPI.CallStatic("setMaxLevel", maxUserLevel);
    }

    public override int GetMaxLevel() {
      return tapjoyJavaAPI.CallStatic<int>("getMaxLevel");
    }

    public override void SetUserSegment(TJSegment userSegment) {
      tjSupportClass.CallStatic("setUserSegment", (int)userSegment);
    }
    public override int GetUserSegment() {
      return tjSupportClass.CallStatic<int>("getUserSegment");
    }

    public override double GetScreenScale() {
      return tjSupportClass.CallStatic<double>("getScreenScale");
    }

    // User Tags
    public override void ClearUserTags() {
      tapjoyJavaAPI.CallStatic("clearUserTags");
    }

    public override List<string> GetUserTags() {
      AndroidJavaObject tagSet = tapjoyJavaAPI.CallStatic<AndroidJavaObject>("getUserTags");
      AndroidJavaObject tagIterator = tagSet.Call<AndroidJavaObject>("iterator");
      List<string> tags = new List<string>();

      while(tagIterator.Call<bool>("hasNext")){
        tags.Add(tagIterator.Call<string>("next"));
      }
      return tags;
    }

    public override void AddUserTag(string tag) {
      tag = MakeStringUnitySafe(tag);
      tapjoyJavaAPI.CallStatic("addUserTag", tag);
    }

    public override void RemoveUserTag(string tag) {
      tag = MakeStringUnitySafe(tag);
      tapjoyJavaAPI.CallStatic("removeUserTag", tag);
    }

   public override void TrackPurchase(string currencyCode, double price) {
      currencyCode = MakeStringUnitySafe(currencyCode);
      tapjoyJavaAPI.CallStatic("trackPurchase", currencyCode, price);
    }

    /** Workaround for bug on null argument passing in Unity below 4.1. */
    private string MakeStringUnitySafe(string s) {
      if (s != null) {
        return s;
      }
      return canPassNullArgument ? null : "";
    }
  }
}

#endif // UNITY_ANDROID
