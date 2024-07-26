using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using TapjoyUnity.Internal.SimpleJSON;

#if UNITY_IOS

namespace TapjoyUnity.Internal {

  public sealed class ApiBindingIos : ApiBinding {

    public static void Install() {
      ApiBinding.SetInstance(new ApiBindingIos());
    }

    ApiBindingIos() : base("iOS") {
      Tapjoy_SetUnityVersion(VERSION_NAME);
    }

    // Connect
    public override void Connect(string sdkKey, Dictionary<string, System.Object> flags) {
      if (sdkKey == null) {
        return;
      }

      if (flags != null) {
        foreach (KeyValuePair<string, System.Object> kvp in flags) {
          // Is the value a dictionary itself
          if (kvp.Value.GetType().IsGenericType) {
            Dictionary<string, System.Object> dictionaryToTransfer = (Dictionary<string, System.Object>) kvp.Value;
            string dictionaryName = kvp.Key;
            // Communicate with objetive-c to build the dictionary
            transferDictionaryToObjectiveCWithName(dictionaryToTransfer, dictionaryName);
            // Tell Objective-C to add the dictionary *dictionaryName* to dictionary "connectFlags" under the key *kvp.key*
            Tapjoy_SetKeyToDictionaryRefValueInDictionary(kvp.Key, dictionaryName, CONNECT_FLAG_DICTIONARY_NAME);
          } else {
            // Tell Objective-C to add the value *kvp.Value* to dictionary "connectFlags" under the key *kvp.Key*
            Tapjoy_SetKeyToValueInDictionary(kvp.Key, kvp.Value.ToString(), CONNECT_FLAG_DICTIONARY_NAME);
          }
        }
      }

      Tapjoy_Connect(sdkKey);
    }

    private void transferDictionaryToObjectiveCWithName(Dictionary<string, System.Object> dictionary, string dictionaryName) {
      foreach (KeyValuePair<string, System.Object> kvp in dictionary) {
        // C# ToString() for bools is either "True" or "False" which is not good -- convert to "true" or "false"
        string dictValue;
        if (kvp.Value.GetType() == typeof(bool))
          dictValue = (Convert.ToBoolean(kvp.Value.ToString())) ? "true" : "false";
        else
          dictValue = kvp.Value.ToString();

        Tapjoy_SetKeyToValueInDictionary(kvp.Key, dictValue, dictionaryName);
      }
    }

    public override void ActionComplete(string actionID) {
      Tapjoy_ActionComplete(actionID);
    }

    // Config
    public override string GetSDKVersion() {
      return Tapjoy_GetSDKVersion();
    }

    public override void SetDebugEnabled(bool enabled) {
      Tapjoy_SetDebugEnabled(enabled);
    }

    public override void GetPrivacyPolicy() {
      Tapjoy_GetPrivacyPolicy();
    }

    public override void SetSubjectToGDPR(TJStatus gdprApplicable) {
      Tapjoy_SetSubjectToGDPRStatus((int)gdprApplicable);
    }

    public override int GetSubjectToGDPR() { 
      return Tapjoy_GetSubjectToGDPRStatus ();
    }

    public override void SetUserConsent(TJStatus consent) {
      Tapjoy_SetUserConsentStatus((int)consent);
    }

    public override int GetUserConsent () {
      return Tapjoy_GetUserConsentStatus();
    }

    public override void SetBelowConsentAge(TJStatus belowConsentAge) {
      Tapjoy_SetBelowConsentAgeStatus((int)belowConsentAge);
    }

    public override int GetBelowConsentAge() {
      return Tapjoy_GetBelowConsentAgeStatus ();
    }

    public override void SetUSPrivacy(string privacyConsent) {
      Tapjoy_SetUSPrivacy(privacyConsent);
    }

    public override string GetUSPrivacy() {
      return Tapjoy_GetUSPrivacy();
    }

    public override void OptOutAdvertisingID(bool optOut) {
      // no op
    }

    public override void ActivateUnitySupport() {
      // figure this out
    }

    public override void GetCurrencyBalance() {
      Tapjoy_GetCurrencyBalance();
    }

    public override void SpendCurrency(int amount) {
      Tapjoy_SpendCurrency(amount);
    }

    public override void AwardCurrency(int amount) {
      Tapjoy_AwardCurrency(amount);
    }

    public override string GetSupportURL() {
      return Tapjoy_GetSupportURL();
    }

    public override string GetSupportURL(string currencyID) {
      return Tapjoy_GetSupportURL2(currencyID);
    }

    public override void ShowDefaultEarnedCurrencyAlert() {
      Tapjoy_ShowDefaultEarnedCurrencyAlert();
    }

    // Tapjoy Placement
    public override void CreatePlacement(string placementGuid, string eventName) {
      Tapjoy_CreatePlacement(placementGuid, eventName);
    }

    public override void DismissPlacementContent() {
      Tapjoy_DismissPlacementContent();
    }

    public override void RequestPlacementContent(string placementGuid) {
      Tapjoy_RequestPlacementContent(placementGuid);
    }

    public override void ShowPlacementContent(string placementGuid) {
      Tapjoy_ShowPlacementContent(placementGuid);
    }

    public override bool IsPlacementContentReady(string placementGuid) {
      return Tapjoy_IsPlacementContentReady(placementGuid);
    }

    public override bool IsPlacementContentAvailable(string placementGuid) {
      return Tapjoy_IsPlacementContentAvailable(placementGuid);
    }

    public override void SetCurrencyBalance (string placementGuid, string currencyId, int balance) {
      Tapjoy_SetPlacementBalance (placementGuid, currencyId, balance);
    }

    public override int GetCurrencyBalance (string placementGuid, string currencyId) {
      return Tapjoy_GetPlacementBalance (placementGuid, currencyId);
    }

    public override void SetRequiredAmount (string placementGuid, string currencyId, int amount) {
      Tapjoy_SetRequiredAmount (placementGuid, currencyId, amount);
    }

    public override int GetRequiredAmount (string placementGuid, string currencyId) {
      return Tapjoy_GetRequiredAmount (placementGuid, currencyId);
    }

    public override void ActionRequestCompleted(string requestId) {
      Tapjoy_ActionRequestCompleted(requestId);
    }

    public override void ActionRequestCancelled(string requestId) {
      Tapjoy_ActionRequestCancelled(requestId);
    }

    public override void RemovePlacement(string placementGuid) {
      Tapjoy_RemovePlacement(placementGuid);
    }

    public override void RequestOfferwallDiscover(string placementName, float height)
    {
      Tapjoy_RequestOfferwallDiscover(placementName, height);
    }

    public override void RequestOfferwallDiscover(string placementName, float left, float top, float width, float height)
    {
      Tapjoy_RequestOfferwallDiscoverAtPosition(placementName, left, top, width, height);
    }

    public override void ShowOfferwallDiscover()
    {
      Tapjoy_ShowOfferwallDiscover();  
    }

    public override void DestroyOfferwallDiscover()
    {
      Tapjoy_DestroyOfferwallDiscover();  
    }

    public override void RemoveActionRequest(string requestID) {
      Tapjoy_RemoveActionRequest(requestID);
    }

    public override void SetEntryPoint(string placementGuid, TJEntryPoint entryPoint) {
      Tapjoy_SetEntryPoint(placementGuid, entryPoint.GetValue());
    }

    // 5Rocks Cohorts
    public override void SetUserID(string userID) {
      Tapjoy_SetUserID(userID);
    }

    public override string GetUserID() {
      return Tapjoy_GetUserID();
    }

    // Currency Callback Param
    public override void SetCustomParameter(string customParam) {
      Tapjoy_SetCustomParameter(customParam);
    }

    public override string GetCustomParameter() {
      return Tapjoy_GetCustomParameter();
    }

    public override void SetUserLevel(int userLevel) {
      Tapjoy_SetUserLevel(userLevel);
    }

    public override int GetUserLevel() {
      return Tapjoy_GetUserLevel();
    }
    
    public override void SetMaxLevel(int maxUserLevel) {
      Tapjoy_SetMaxLevel(maxUserLevel);
    }

    public override int GetMaxLevel() {
      return Tapjoy_GetMaxLevel();
    }

    public override void SetUserSegment(TJSegment userSegment) {
      Tapjoy_SetUserSegment(userSegment);
    }

    public override int GetUserSegment() {
      return (int)Tapjoy_GetUserSegment();
    }

    public override double GetScreenScale() {
      return Tapjoy_GetScreenScale();
    }

    // User Tags
    public override void ClearUserTags() {
      Tapjoy_ClearUserTags();
    }

    public override List<string> GetUserTags() {
      string serializedTags = Tapjoy_GetUserTags();
      JSONArray tags = JSON.Parse(serializedTags).AsArray;
      List<string> tagList = new List<string>();
      for(int i=0; i<tags.Count; i++){
        tagList.Add(tags[i]);
      }
      return tagList;
    }

    public override void AddUserTag(string tag) {
      Tapjoy_AddUserTag(tag);
    }

    public override void RemoveUserTag(string tag) {
      Tapjoy_RemoveUserTag(tag);
    }

    public override void TrackPurchase(string currencyCode, double price) {
      Tapjoy_TrackPurchase(currencyCode, price);
    }

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetUnityVersion(string version);

    [DllImport("__Internal")]
    private static extern void Tapjoy_Connect(string sdkKey);

    [DllImport("__Internal")]
    private static extern void Tapjoy_ActionComplete(string actionID);

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetKeyToValueInDictionary(string key, string valueToSet, string dictionaryName);

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetKeyToDictionaryRefValueInDictionary(string key, string dictionaryNameToSet, string dictionaryNameToSetTo);

    [DllImport("__Internal")]
    private static extern string Tapjoy_GetSDKVersion();

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetDebugEnabled(bool enabled);

    [DllImport("__Internal")]
    private static extern void Tapjoy_GetPrivacyPolicy();

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetSubjectToGDPRStatus(int gdprApplicable);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetSubjectToGDPRStatus ();

    [DllImport ("__Internal")]
    private static extern void Tapjoy_SetUserConsentStatus (int consent);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetUserConsentStatus ();

    [DllImport ("__Internal")]
    private static extern void Tapjoy_SetBelowConsentAgeStatus (int belowConsentAge);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetBelowConsentAgeStatus ();

    [DllImport ("__Internal")]
    private static extern void Tapjoy_SetUSPrivacy(string privacyConsent);

    [DllImport ("__Internal")]
    private static extern string Tapjoy_GetUSPrivacy();

    [DllImport("__Internal")]
    private static extern void Tapjoy_GetCurrencyBalance();

    [DllImport("__Internal")]
    private static extern void Tapjoy_SpendCurrency(int amount);

    [DllImport("__Internal")]
    private static extern void Tapjoy_AwardCurrency(int amount);

    [DllImport("__Internal")]
    private static extern string Tapjoy_GetSupportURL();

    [DllImport("__Internal")]
    private static extern string Tapjoy_GetSupportURL2(string currencyID);

    [DllImport("__Internal")]
    private static extern void Tapjoy_ShowDefaultEarnedCurrencyAlert();

    [DllImport("__Internal")]
    private static extern void Tapjoy_CreatePlacement(string placementGuid, string eventName);

    [DllImport("__Internal")]
    private static extern void Tapjoy_DismissPlacementContent();

    [DllImport("__Internal")]
    private static extern void Tapjoy_RequestPlacementContent(string placementGuid);

    [DllImport("__Internal")]
    private static extern void Tapjoy_ShowPlacementContent(string placementGuid);

    [DllImport("__Internal")]
    private static extern bool Tapjoy_IsPlacementContentAvailable(string placementGuid);

    [DllImport("__Internal")]
    private static extern bool Tapjoy_IsPlacementContentReady(string placementGuid);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_SetPlacementBalance (string placementGuid, string currencyId, int amount);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetPlacementBalance (string placementGuid, string currencyId);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_SetRequiredAmount (string placementGuid, string currencyId, int amount);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetRequiredAmount (string placementGuid, string currencyId);

    [DllImport ("__Internal")]
    private static extern bool Tapjoy_RequestOfferwallDiscover(string placementName, float height);

    [DllImport ("__Internal")]
    private static extern bool Tapjoy_RequestOfferwallDiscoverAtPosition(string placementName, float left, float top, float width, float height);

    [DllImport ("__Internal")]
    private static extern bool Tapjoy_ShowOfferwallDiscover();

    [DllImport("__Internal")]
    private static extern bool Tapjoy_DestroyOfferwallDiscover();

    [DllImport("__Internal")]
    private static extern void Tapjoy_ActionRequestCompleted(string requestId);

    [DllImport("__Internal")]
    private static extern void Tapjoy_ActionRequestCancelled(string requestId);

    [DllImport("__Internal")]
    private static extern void Tapjoy_RemovePlacement(string placementGuid);

    [DllImport("__Internal")]
    private static extern void Tapjoy_RemoveActionRequest(string requestId);

    [DllImport ("__Internal")]
    private static extern void Tapjoy_SetEntryPoint(string placementGuid, string entryPoint);

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetUserID(string userId);

    [DllImport("__Internal")]
    private static extern string Tapjoy_GetUserID();

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetCustomParameter(string customParam);

    [DllImport("__Internal")]
    private static extern string Tapjoy_GetCustomParameter();

    [DllImport("__Internal")]
    private static extern void Tapjoy_SetUserLevel(int userLevel);
    
    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetUserLevel ();

    [DllImport ("__Internal")]
    private static extern void Tapjoy_SetMaxLevel (int maxLevel);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetMaxLevel ();

    [DllImport ("__Internal")]
    private static extern void Tapjoy_SetUserSegment (TJSegment userSegment);

    [DllImport ("__Internal")]
    private static extern int Tapjoy_GetUserSegment ();

    [DllImport ("__Internal")]
    private static extern double Tapjoy_GetScreenScale ();

    [DllImport("__Internal")]
    private static extern void Tapjoy_ClearUserTags();

    [DllImport("__Internal")]
    private static extern string Tapjoy_GetUserTags();

    [DllImport("__Internal")]
    private static extern void Tapjoy_AddUserTag(string tag);

    [DllImport("__Internal")]
    private static extern void Tapjoy_RemoveUserTag(string tag);

    [DllImport("__Internal")]
    private static extern void Tapjoy_TrackPurchase(string currencyCode, double price);

    private static string GetStringFromNativeUtf8(IntPtr nativeUtf8) {
      int len = 0;
      while (Marshal.ReadByte(nativeUtf8, len) != 0) {
        len++;
      }
      if (len == 0) {
        return string.Empty;
      }
      byte[] buffer = new byte[len];
      Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
      return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
    }
  }
}

#endif // UNITY_IOS
