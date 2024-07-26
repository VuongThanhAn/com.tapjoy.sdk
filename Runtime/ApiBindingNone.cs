using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace TapjoyUnity.Internal {

  internal class ApiBindingNone : ApiBinding {

    public ApiBindingNone() : base("None") {

    }

    // Connect
    public override void Connect(string sdkKey, Dictionary<string, System.Object> flag) {

    }

    public override void ActionComplete(string actionID) {
    }

    // Config
    public override string GetSDKVersion() {
      return "none";
    }

    public override void SetDebugEnabled(bool enabled) {
    }

    public override void ActivateUnitySupport() {
    }

    // Tapjoy
    public override void GetPrivacyPolicy(){

    }

    public override void SetSubjectToGDPR(TJStatus subject) {

    }

    public override int GetSubjectToGDPR () {
      return (int)TJStatus.UNKNOWN;
    }

    public override void SetUserConsent(TJStatus consent) {

    }

    public override int GetUserConsent() {
        return (int)TJStatus.UNKNOWN;
    }

    public override void SetBelowConsentAge(TJStatus isBelowConsentAge) {

    }

    public override int GetBelowConsentAge () {
      return (int)TJStatus.UNKNOWN;
    }

    public override void SetUSPrivacy(string privacyConsent) {

    }

    public override string GetUSPrivacy() {
      return "";
    }

    public override void OptOutAdvertisingID(bool optOut) {

    }
    
    public override void GetCurrencyBalance() {

    }

    public override void SpendCurrency(int amount) {

    }

    public override void AwardCurrency(int amount) {

    }

    public override string GetSupportURL() {
        return "";
    }

    public override string GetSupportURL(string currencyID) {
        return "";
    }

    public override void ShowDefaultEarnedCurrencyAlert() {

    }

    // Tapjoy Placement
    public override void CreatePlacement(string placementGuid, string eventName) {

    }

    public override void DismissPlacementContent() {
    }

    public override void RequestPlacementContent(string placementGuid) {

    }

    public override void ShowPlacementContent(string placementGuid) {

    }

    public override bool IsPlacementContentReady(string placementGuid) {
      return false;
    }

    public override bool IsPlacementContentAvailable(string placementGuid) {
      return false;
    }

    public override void SetCurrencyBalance (string placementGuid, string currencyId, int balance) {
    }

    public override int GetCurrencyBalance (string placementGuid, string currencyId) {
      return -1;
    }

    public override void SetRequiredAmount (string placementGuid, string currencyId, int amount) {
    }

    public override int GetRequiredAmount (string placementGuid, string currencyId) {
      return -1;
    }

    public override void ActionRequestCompleted(string requestId) {

    }

    public override void ActionRequestCancelled(string requestId) {

    }

    public override void RemovePlacement(string placementGuid) {

    }

    public override void RequestOfferwallDiscover(string placementName, float height) {

    }

    public override void RequestOfferwallDiscover(string placementName, float left, float top, float width, float height)
    {

    }

    public override void ShowOfferwallDiscover() {

    }


    public override void DestroyOfferwallDiscover() {

    }

    public override void RemoveActionRequest(string requestID) {

    }

    public override void SetEntryPoint(string placementGuid, TJEntryPoint entryPoint) {

    }

    // 5Rocks Cohorts
    public override void SetUserID(string userId) {

    }

    public override string GetUserID() {
      return "";
    }

    // Currency Callback Param
    public override void SetCustomParameter(string customParam) {

    }

    public override string GetCustomParameter() {
      return "";
    }

    public override void SetUserLevel(int userLevel) {

    }

    public override int GetUserLevel() {
      return -1;
    }

    public override void SetMaxLevel(int maxUserLevel) {

    }

    public override int GetMaxLevel () {
      return -1;
    }

    public override void SetUserSegment(TJSegment userSegment) {

    }

    public override int GetUserSegment() {
      return -1;
    }

    public override double GetScreenScale() {
      return 1.0;
    }

    // User Tags
    public override void ClearUserTags() {
    }

    public override List<string> GetUserTags() {
      return null;
    }

    public override void AddUserTag(string tag) {
    }

    public override void RemoveUserTag(string tag) {
    }

    public override void TrackPurchase(string currencyCode, double price) {
    }
  }
}
