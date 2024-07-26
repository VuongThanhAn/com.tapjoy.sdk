using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using TapjoyUnity.Internal;

namespace TapjoyUnity {

  public class Tapjoy {
    internal const string VERSION_NAME = "14.0.1";
    internal const string VERSION_SUFFIX = "";

    /**
     * @brief Returns the version name of the SDK.
     * @return a string which represents the version name
     */
    public static string Version {
      get {
        if (String.IsNullOrEmpty(VERSION_SUFFIX)) {
          return VERSION_NAME;
        } else {
          return String.Format("{0}-{1}", VERSION_NAME, VERSION_SUFFIX);
        }
      }
    }

    private static bool _isConnected = false;

    /**
     * @brief Checks if SDK is connected
     * @return true if SDK is connected
     *         false otherwise
     */
    public static bool IsConnected {
      get {
        return _isConnected;
      }
      set {
        _isConnected = value;
      }
    }

    /**
     * @brief Connect to the Tapjoy SDK
     * If Tapjoy fails to connect due to a network issue,
     * you can try connecting later by yourself using this method.
     * This call will reuse the same paramters from the last connect call.
     * The result of the call is passed via OnConnectSuccess or OnConnectFailed.
     */
    public static void Connect() {
      TapjoyComponent component = TapjoyComponent.FindInstance();
      if (component == null) {
        Debug.LogWarning("Can't connect. Tapjoy object is missing.");
        return;
      }
      component.Reconnect();
    }

    /**
     * @brief Connect to the Tapjoy SDK with Tapjoy sdkKey.
     * Make sure to pass in the correct sdkKey based on platform.
     * The result of the call is passed via OnConnectSuccess or OnConnectFailed.
     *
     * @param sdkKey
     *        Tapjoy sdkKey to use for this connect
     */
    public static void Connect(string sdkKey) {
      TapjoyComponent component = TapjoyComponent.FindInstance();
      if (component == null) {
        Debug.LogWarning("Can't connect. Tapjoy object is missing.");
        return;
      }
      component.ConnectManually(sdkKey);
    }

    /**
     * @brief Connect to the Tapjoy SDK with Tapjoy sdkKey.
     * Make sure to pass in the correct sdkKey based on platform.
     * Connect settings are defaulty taken from the Tapjoy Unity
     * Editor window, but can be overriden / set dynamically using connectFlags.
     * The result of the call is passed via OnConnectSuccess or OnConnectFailed.
     *
     * @param sdkKey
     *        Tapjoy sdkKey to use for this connect
     */
    public static void Connect(string sdkKey, Dictionary<string, System.Object> connectFlags) {
      TapjoyComponent component = TapjoyComponent.FindInstance();
      if (component == null) {
        Debug.LogWarning("Can't connect. Tapjoy object is missing.");
        return;
      }
      component.ConnectManually(sdkKey, connectFlags);
    }

    /**
     * @brief Enables the debug mode of the SDK.
     * @param enable
     *        set to true if logging should be enabled
     *        false to disable logging
     */
    public static void SetDebugEnabled(bool enable) {
      ApiBinding.Instance.SetDebugEnabled(enable);
    }


    /**
     * @brief Restricts access to Google AdvertisingID
     * @param optOut True will prevent Tapjoy from accessing the devices AdvertisingID, False (default) will allow access
     */
    public static void OptOutAdvertisingID(bool optOut) {
      ApiBinding.Instance.OptOutAdvertisingID(optOut);
    }
 
    /**
     * @brief Informs the Tapjoy server that the specified Pay-Per-Action
     * was completed. Should be called whenever a user completes an in-game action.
     * @param actionID
     *        The action ID of the completed action
     */
    public static void ActionComplete(string actionID) {
      ApiBinding.Instance.ActionComplete(actionID);
    }

    //////////////////////////////////////////////////
    // App and User Properties
    //////////////////////////////////////////////////

    /**
     * @brief Sets the identifier of the user.
     * @param userId
     *        the identifier of the user
     */
    public static void SetUserID(string userId) {
      ApiBinding.Instance.SetUserID(userId);
    }

    /**
     * @brief Gets the identifier of the user.
     *
     * @return the value of the identifier
     */
    public static string GetUserID() {
      return ApiBinding.Instance.GetUserID();
    }

#if !DOXYGEN_SHOULD_SKIP_THIS
 /* code that must be skipped by doxygen */
 // Excluding custom parameters from docs until supported by backend

    /**
     * Assigns a custom parameter associated with any following placement requests that contains an ad type.
     * We will return this value on the currency callback. Only applicable for publishers who manage their own currency servers.
     * This value does NOT get unset with each subsequent placement request.
     *
     * @param customParam The custom parameter to assign to this device
     * @return n/a
     */
    public static void SetCustomParameter(string customParam) {
      ApiBinding.Instance.SetCustomParameter(customParam);
    }

    /**
    * Returns the currently set custom parameter.
    *
    * @return the value of the currently set custom parameter
    */
    public static string GetCustomParameter() {
      return ApiBinding.Instance.GetCustomParameter();
    }
#endif

    /**
     * @brief Sets the level of the user.
     * @param userLevel
     *        the level of the user
     */
    public static void SetUserLevel(int userLevel) {
      ApiBinding.Instance.SetUserLevel(userLevel);
    }

    /**
    * Returns the level of the user.
    *
    * @return the value of the user level
    */
    public static int GetUserLevel() {
      return ApiBinding.Instance.GetUserLevel();
    }

    /**
     * @brief Sets the maximum level of the user.
     * @param maxUserLevel
	   *        the maximum level
     */
    public static void SetMaxLevel (int maxUserLevel) {
      ApiBinding.Instance.SetMaxLevel(maxUserLevel);
    }

    /**
     * @brief Gets the maximum level of the user.
     * @return the maximum level
     */
    public static int GetMaxLevel() {
      return ApiBinding.Instance.GetMaxLevel();
    }

    public static void SetUserSegment(TJSegment userSegment) {
      ApiBinding.Instance.SetUserSegment(userSegment);
    }

    public static TJSegment GetUserSegment() {
      return (TJSegment)ApiBinding.Instance.GetUserSegment();
    }

    public static double GetScreenScale() {
      return ApiBinding.Instance.GetScreenScale();
    }

    //////////////////////////////////////////////////
    // User Tags
    //////////////////////////////////////////////////

    /**
     * @brief Removes all tags from the user.
     */
    public static void ClearUserTags() {
      ApiBinding.Instance.ClearUserTags();
    }

    /**
     * @brief Get all tags from the user.
     */
    public static List<string> GetUserTags() {
      return ApiBinding.Instance.GetUserTags();
    }

    /**
     * @brief Adds the given tag to the user if it is not already present.
     *
     * @param tag
     *        the tag to be added
     */
    public static void AddUserTag(string tag) {
      ApiBinding.Instance.AddUserTag(tag);
    }

    /**
     * @brief Removes the given tag from the user if it is present.
     *
     * @param tag
     *        the tag to be removed
     */
    public static void RemoveUserTag(string tag) {
      ApiBinding.Instance.RemoveUserTag(tag);
    }

    /**
     * @brief Tracks the purchase.
     * @param currencyCode
     *        the currency code of price as an alphabetic currency code specified
     *        in ISO 4217, i.e. "USD", "KRW"
     * @param productPrice
     *        the price of product
     */
    public static void TrackPurchase(string currencyCode, double productPrice) {
      ApiBinding.Instance.TrackPurchase(currencyCode, productPrice);
    }

    /**
     * @deprecated Deprecated since version 14.0.0
     * @brief Tracks the purchase.
     * @param productId
     *        the product identifier
     * @param currencyCode
     *        the currency code of price as an alphabetic currency code specified
     *        in ISO 4217, i.e. "USD", "KRW"
     * @param productPrice
     *        the price of product
     * @param campaignId
     *        the campaign id of the purchase ActionRequest which initiated this
     *        purchase, can be null
     */
    public static void TrackPurchase(string productId, string currencyCode, double productPrice, string campaignId = null) {
      ApiBinding.Instance.TrackPurchase(currencyCode, productPrice);
    }

    /**
     * @deprecated Deprecated since version 14.0.0
     * @brief Tracks a purchase with JSON data from the Google Play store.
     *        Also performs In-app Billing validation if purchaseData and dataSignature are given.
     *
     * @param skuDetails
     *        a String in JSON Object format that contains product item
     *        details (according to <a href=
     *        "http://developer.android.com/google/play/billing/billing_reference.html#product-details-table"
     *        >Specification on Google Play</a>)
     * @param purchaseData
     *        a String in JSON format that contains details about the purchase order.
     *        Use null not to use validation.
     * @param dataSignature
     *        String containing the signature of the purchase data that the developer signed with their private key.
     *        Use null not to use validation.
     * @param campaignId
     *        the campaign id of the Purchase Action Request if it initiated
     *        this purchase, can be null
     */
    public static void TrackPurchaseInGooglePlayStore(string skuDetails, string purchaseData, string dataSignature, string campaignId = null) {
      //no-op
    }

    /**
     * @deprecated Deprecated since version 14.0.0
     * @brief Tracks a purchase from the Apple App Store.
     *
     * @param productId
     *        the identifier of product
     * @param currencyCode
     *        the currency code of price as an alphabetic currency code specified in ISO 4217, i.e. "USD", "KRW"
     * @param price
     *        the price of product
     * @param transactionId
     *        the identifier of iap transaction,
     *        if this is given, we will check receipt validation. (Available in iOS 7.0 and later)
     * @param campaignId
     *        the campaign id of the purchase request which initiated this purchase, can be null
     */
    public static void TrackPurchaseInAppleAppStore(string productId, string currencyCode, double productPrice, string transactionId, string campaignId = null) {
      ApiBinding.Instance.TrackPurchase(currencyCode, productPrice);
    }

    #region Currency Bridge Calls (C# -> Native)

    /**
     * @brief Awards virtual currency.
     *        This can only be used for currency managed by Tapjoy.
     *        The data will be delivered to the delegates
     *        subscribing #OnAwardCurrencyResponse and #OnAwardCurrencyResponseFailure
     * @param amount
     *        Amount of the currency
     */
    public static void AwardCurrency(int amount) {
      ApiBinding.Instance.AwardCurrency(amount);
    }

    /**
     * @brief Gets the virtual currency data from the server for this device.
     *        The data will be delivered to the delegates
     *        subscribing #OnGetCurrencyBalanceResponse and #OnGetCurrencyBalanceResponseFailure
     */
    public static void GetCurrencyBalance() {
      ApiBinding.Instance.GetCurrencyBalance();
    }

    /**
     * @brief Spends virtual currency.
     *        This can only be used for currency managed by Tapjoy.
     *        The data will be delivered to the delegates
     *        subscribing #OnSpendCurrencyResponse and #OnSpendCurrencyResponseFailure
     * @param amount
     *        Amount of the currency
     */
    public static void SpendCurrency(int amount) {
      ApiBinding.Instance.SpendCurrency(amount);
    }

    /**
     * @brief Returns URL to Tapjoy support web page. This will use your default currency.
     *
     * @return URL of Tapjoy support web page
     */
    public static string GetSupportURL() {
      return ApiBinding.Instance.GetSupportURL();
    }

    /**
     * @brief Returns URL to Tapjoy support web page for specified currency
     * You can get your currencyId from the Tapjoy Dashboard under the currency section.
     *
     * @param currencyId
     *      the app's currency id
     *
     * @return URL of Tapjoy support web page for specified currency
     */
    public static string GetSupportURL(string currencyID) {
      return ApiBinding.Instance.GetSupportURL(currencyID);
    }

    /**
     * @brief Shows default alert that tells the user how much currency they just earned.
     */
    public static void ShowDefaultEarnedCurrencyAlert() {
      ApiBinding.Instance.ShowDefaultEarnedCurrencyAlert();
    }

    #endregion

    #region Connect Delegates

    /**
     * @brief Delegate to be called when the SDK is connected
     */
    public delegate void OnConnectSuccessHandler();

    private static OnConnectSuccessHandler OnConnectSuccessInvoker;
    private static OnConnectSuccessHandler OnConnectSuccessInternalInvoker;

    /**
     * @brief Event for #OnConnectSuccessHandler
     */
    public static event OnConnectSuccessHandler OnConnectSuccess {
      add {
        OnConnectSuccessInvoker += value;
      }
      remove {
        OnConnectSuccessInvoker -= value;
      }
    }

    internal static event OnConnectSuccessHandler OnConnectSuccessInternal {
      add {
        OnConnectSuccessInternalInvoker += value;
      }
      remove {
        OnConnectSuccessInternalInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when the SDK fails to connect
     * @param code The error code.
     * @param message The error message.
    */
    public delegate void OnConnectFailedHandler (int code, string message);

    private static OnConnectFailedHandler OnConnectFailedInvoker;
    private static OnConnectFailedHandler OnConnectFailedInternalInvoker;

    /**
     * @brief Event for #OnConnectFailedHandler
     */
    public static event OnConnectFailedHandler OnConnectFailed {
      add {
        OnConnectFailedInvoker += value;
      }
      remove {
        OnConnectFailedInvoker -= value;
      }
    }

    internal static event OnConnectFailedHandler OnConnectFailedInternal {
      add {
        OnConnectFailedInternalInvoker += value;
      }
      remove {
        OnConnectFailedInternalInvoker -= value;
      }
    }
    
  /**
   * @brief Delegate to be called when the SDK returns a warning
   * @param code The warning code.
   * @param message The warning message.
   */
    public delegate void OnConnectWarningHandler(int code, string message);

    private static OnConnectWarningHandler OnConnectWarningInvoker;
    private static OnConnectWarningHandler OnConnectWarningInternalInvoker;

    /**
     * @brief Event for #OnConnectWarningHandler
     */
    public static event OnConnectWarningHandler OnConnectWarning {
      add {
        OnConnectWarningInvoker += value;
      }
      remove {
        OnConnectWarningInvoker -= value;
      }
    }
    
    internal static event OnConnectWarningHandler OnConnectWarningInternal {
      add {
        OnConnectWarningInternalInvoker += value;
      }
      remove {
        OnConnectWarningInternalInvoker -= value;
      }
    }

    #endregion

    #region Connect Bridge Calls (Native -> C#)

    internal static void DispatchConnectEvent(string connectCallbackMethod) {
      #if DEBUG
      Debug.Log("TapjoyUnity.DispatchConnectEvent(" + connectCallbackMethod + ")");
#endif
     if (connectCallbackMethod.Equals ("OnConnectSuccess")) {
        _isConnected = true;
        OnConnectSuccessInternalInvoker?.Invoke ();
        OnConnectSuccessInvoker?.Invoke ();
      } else if (connectCallbackMethod.Contains ("OnConnectFailure")) {
        string [] args = connectCallbackMethod.Split (',');
        if (args.Length < 3) {
#if DEBUG
          Debug.Log ("TapjoyUnity.DispatchConnectEvent: Wrong callback length");
#endif
          return;
        }
        string errorCodeArgs = args [1];
        string errorMessageArgs = args [2];
        try {
          int errorCode = Int32.Parse (errorCodeArgs);
          OnConnectFailedInternalInvoker?.Invoke (errorCode, errorMessageArgs);
          OnConnectFailedInvoker?.Invoke (errorCode, errorMessageArgs);
        } catch {
#if DEBUG
          Debug.Log ("TapjoyUnity.DispatchConnectEvent errorCode parse error");
#endif
        }
      } else if (connectCallbackMethod.Contains("OnConnectWarning")) {
        string[] args = connectCallbackMethod.Split(',');
        if (args.Length < 3) {
          Debug.Log("TapjoyUnity.DispatchConnectEvent: Wrong callback length");
          return;
        }
         try {
            int warningCode = Int32.Parse(args[1]);
            string warningMessage = args[2];
            OnConnectWarningInternalInvoker?.Invoke(warningCode, warningMessage);
            OnConnectWarningInvoker?.Invoke(warningCode, warningMessage);
          } catch {
#if DEBUG
            Debug.Log("TapjoyUnity.DispatchConnectEvent warning parse error");
#endif
          }
      }
    }

    #endregion

    #region Set User ID Delegates

    /**
     * @brief Delegate to be called when the SDK has set User ID
     */
    public delegate void OnSetUserIDSuccessHandler();

    private static OnSetUserIDSuccessHandler OnSetUserIDSuccessInvoker;

    /**
     * @brief Event for #OnSetUserIDSuccessHandler
     */
    public static event OnSetUserIDSuccessHandler OnSetUserIDSuccess {
      add {
        OnSetUserIDSuccessInvoker += value;
      }
      remove {
        OnSetUserIDSuccessInvoker -= value;
      }
    }
    
   /**
    * @brief Delegate to be called when the SDK fails to set User ID
    * @param code The code of the error
    * @param errorMessage The message of the error
    */
    public delegate void OnSetUserIDFailedHandler(int code, string errorMessage);

    private static OnSetUserIDFailedHandler OnSetUserIDFailedInvoker;

    /**
     * @brief Event for #OnSetUserIDFailedHandler
     */
    public static event OnSetUserIDFailedHandler OnSetUserIDFailed {
      add {
        OnSetUserIDFailedInvoker += value;
      }
      remove {
        OnSetUserIDFailedInvoker -= value;
      }
    }
    
    /**
     * @deprecated Deprecated since version 13.4.0 
     * @brief Delegate to be called when the SDK fails to set User ID
     */
    [Obsolete ("Deprecated since 13.4.0.")]
    public delegate void OnSetUserIDFailureHandler(string errorMessage);

    private static OnSetUserIDFailureHandler OnSetUserIDFailureInvoker;

    /**
     * @deprecated Deprecated since version 13.4.0 
     * @brief Event for #OnSetUserIDFailureHandler
     */
    [Obsolete ("Deprecated since 13.4.0.")]
    public static event OnSetUserIDFailureHandler OnSetUserIDFailure {
      add {
        OnSetUserIDFailureInvoker += value;
      }
      remove {
        OnSetUserIDFailureInvoker -= value;
      }
    }

    #endregion

    #region Set User ID Bridge Calls (Native -> C#)

    internal static void DispatchSetUserIDEvent(string commaDelimitedMessage) {
      #if DEBUG
      Debug.Log("TapjoyUnity.DispatchSetUserIDEvent(" + commaDelimitedMessage + ")");
      #endif
      string[] args = commaDelimitedMessage.Split(',');
      switch (args[0]) {
      case "OnSetUserIDSuccess":
        {
          if (OnSetUserIDSuccessInvoker != null) {
            OnSetUserIDSuccessInvoker();
          }
          break;
        }
      case "OnSetUserIDFailure":
        {
          if (args.Length < 2) {
            Debug.Log("TapjoyUnity.DispatchSetUserIDEvent: Wrong callback length");
          } else if (args.Length == 2) {
            string errorMessage = args[1];
            OnSetUserIDFailureInvoker?.Invoke(errorMessage);
          } else if (args.Length > 2) {
            try {
              int errorCode = Int32.Parse (args[1]);
              string errorMessage = args[2];
              OnSetUserIDFailedInvoker?.Invoke (errorCode, errorMessage);
            } catch {
#if DEBUG
              Debug.Log ("TapjoyUnity.DispatchSetUserIDEvent errorCode parse error");
#endif
            }
          }
          break;
        }
      }
    }

    #endregion

    #region Currency Delegates

    /**
     * @brief Delegate to be called with virtual currency name and total balance information
     *        when #GetCurrencyBalance is successful
     * @param currencyName
     *        The name of the virtual currency.
     * @param balance
     *        Currency balance.
     */
    public delegate void OnGetCurrencyBalanceResponseHandler(string currencyName, int balance);

    private static OnGetCurrencyBalanceResponseHandler OnGetCurrencyBalanceResponseInvoker;

    /**
     * @brief Event for #OnGetCurrencyBalanceResponseHandler
     */
    public static event OnGetCurrencyBalanceResponseHandler OnGetCurrencyBalanceResponse {
      add {
        OnGetCurrencyBalanceResponseInvoker += value;
      }
      remove {
        OnGetCurrencyBalanceResponseInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be notified error message if #GetCurrencyBalance fails
     * @param errorMessage
     *        The error message for a failed request
     */
    public delegate void OnGetCurrencyBalanceResponseFailureHandler(string errorMessage);

    private static OnGetCurrencyBalanceResponseFailureHandler OnGetCurrencyBalanceResponseFailureInvoker;

    /**
     * @brief Event for #OnGetCurrencyBalanceResponseFailureHandler
     */
    public static event OnGetCurrencyBalanceResponseFailureHandler OnGetCurrencyBalanceResponseFailure {
      add {
        OnGetCurrencyBalanceResponseFailureInvoker += value;
      }
      remove {
        OnGetCurrencyBalanceResponseFailureInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called with virtual currency name and total balance information
     *        when #SpendCurrency is successful
     * @param currencyName
     *        The name of the virtual currency.
     * @param balance
     *        Currency balance.
     */
    public delegate void OnSpendCurrencyResponseHandler(string currencyName, int balance);

    private static OnSpendCurrencyResponseHandler OnSpendCurrencyResponseInvoker;

    /**
     * @brief Event for #OnSpendCurrencyResponseHandler
     */
    public static event OnSpendCurrencyResponseHandler OnSpendCurrencyResponse {
      add {
        OnSpendCurrencyResponseInvoker += value;
      }
      remove {
        OnSpendCurrencyResponseInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be notified error message if #SpendCurrency fails
     * @param errorMessage
     *        The error message for a failed request
     */
    public delegate void OnSpendCurrencyResponseFailureHandler(string errorMessage);

    private static OnSpendCurrencyResponseFailureHandler OnSpendCurrencyResponseFailureInvoker;

    /**
     * @brief Event for #OnSpendCurrencyResponseFailureHandler
     */
    public static event OnSpendCurrencyResponseFailureHandler OnSpendCurrencyResponseFailure {
      add {
        OnSpendCurrencyResponseFailureInvoker += value;
      }
      remove {
        OnSpendCurrencyResponseFailureInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called with virtual currency name and total balance information
     *        when #AwardCurrency is successful
     * @param currencyName
     *        The name of the virtual currency.
     * @param balance
     *        Currency balance.
     */
    public delegate void OnAwardCurrencyResponseHandler(string currencyName, int balance);

    private static OnAwardCurrencyResponseHandler OnAwardCurrencyResponseInvoker;

    /**
     * @brief Event for #OnAwardCurrencyResponseHandler
     */
    public static event OnAwardCurrencyResponseHandler OnAwardCurrencyResponse {
      add {
        OnAwardCurrencyResponseInvoker += value;
      }
      remove {
        OnAwardCurrencyResponseInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be notified error message if #AwardCurrency fails
     * @param errorMessage
     *        The error message for a failed request
     */
    public delegate void OnAwardCurrencyResponseFailureHandler(string errorMessage);

    private static OnAwardCurrencyResponseFailureHandler OnAwardCurrencyResponseFailureInvoker;

    /**
     * @brief Event for #OnAwardCurrencyResponseFailureHandler
     */
    public static event OnAwardCurrencyResponseFailureHandler OnAwardCurrencyResponseFailure {
      add {
        OnAwardCurrencyResponseFailureInvoker += value;
      }
      remove {
        OnAwardCurrencyResponseFailureInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called whenever virtual currency has been earned.
     *        This can get called on a #GetCurrencyBalance call.
     * @param currencyName
     *        Virtual currency name
     * @param amount
     *        Amount of virtual currency earned (delta).
     */
    public delegate void OnEarnedCurrencyHandler(string currencyName, int amount);

    private static OnEarnedCurrencyHandler OnEarnedCurrencyInvoker;

    /**
     * @brief Event for #OnEarnedCurrencyHandler
     */
    public static event OnEarnedCurrencyHandler OnEarnedCurrency {
      add {
        OnEarnedCurrencyInvoker += value;
      }
      remove {
        OnEarnedCurrencyInvoker -= value;
      }
    }

    #endregion

    #region Currency Calls (Native -> C#)

    internal static void DispatchCurrencyEvent(string commaDelimitedMessage) {
      #if DEBUG
      Debug.Log("TapjoyUnity.DispatchCurrencyEvent(" + commaDelimitedMessage + ")");
      #endif
      string[] args = commaDelimitedMessage.Split(',');
      switch (args[0]) {
      case "OnGetCurrencyBalanceResponse":
        {
          if (args.Length != 3) {
            return;
          }
          string currencyName = args[1];
          int balance;
          if (int.TryParse(args[2], out balance)) {
            if (OnGetCurrencyBalanceResponseInvoker != null) {
              OnGetCurrencyBalanceResponseInvoker(currencyName, balance);
            }
          }
          break;
        }
      case "OnGetCurrencyBalanceResponseFailure":
        {
          if (args.Length < 2) {
            return;
          }

          string errorMessage = args[1];

          // In case error messages included "," character. TODO: use json
          if (args.Length > 2) {
            for (int i = 2; i < args.Length; i++) {
              errorMessage += args[i];
            }
          }
          if (OnGetCurrencyBalanceResponseFailureInvoker != null) {
            OnGetCurrencyBalanceResponseFailureInvoker(errorMessage);
          }
          break;
        }
      case "OnSpendCurrencyResponse":
        {
          if (args.Length != 3) {
            return;
          }

          string currencyName = args[1];
          int balance;
          if (int.TryParse(args[2], out balance)) {
            if (OnSpendCurrencyResponseInvoker != null) {
              OnSpendCurrencyResponseInvoker(currencyName, balance);
            }
          }
          break;
        }
      case "OnSpendCurrencyResponseFailure":
        {
          if (args.Length < 2) {
            return;
          }

          string errorMessage = args[1];

          // In case error messages included "," character. TODO: use json
          if (args.Length > 2) {
            for (int i = 2; i < args.Length; i++) {
              errorMessage += args[i];
            }
          }
          if (OnSpendCurrencyResponseFailureInvoker != null) {
            OnSpendCurrencyResponseFailureInvoker(errorMessage);
          }
          break;
        }
      case "OnAwardCurrencyResponse":
        {
          if (args.Length != 3) {
            return;
          }
          string currencyName = args[1];
          int balance;
          if (int.TryParse(args[2], out balance)) {
            if (OnAwardCurrencyResponseInvoker != null) {
              OnAwardCurrencyResponseInvoker(currencyName, balance);
            }
          }
          break;
        }
      case "OnAwardCurrencyResponseFailure":
        {
          if (args.Length != 2) {
            return;
          }

          string errorMessage = args[1];

          // In case error messages included "," character. TODO: use json
          if (args.Length > 2) {
            for (int i = 2; i < args.Length; i++) {
              errorMessage += args[i];
            }
          }

          if (OnAwardCurrencyResponseFailureInvoker != null) {
            OnAwardCurrencyResponseFailureInvoker(errorMessage);
          }
          break;
        }
      case "OnEarnedCurrency":
        {
          if (args.Length != 3) {
            return;
          }

          string currencyName = args[1];
          int amount;
          if (int.TryParse(args[2], out amount)) {
            if (OnEarnedCurrencyInvoker != null) {
              OnEarnedCurrencyInvoker(currencyName, amount);
            }
          }
          break;
        }
      }
    }

    #endregion
  }
}
