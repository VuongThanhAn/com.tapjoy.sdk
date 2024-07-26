using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TapjoyUnity.Internal;

namespace TapjoyUnity {

  /**
   * @brief Placement
   */
  public class TJPlacement {
    private static Dictionary<string, WeakReference> placementDictionary = new Dictionary<string, WeakReference>();
    private string _placementName;
    private string _guid;

    private TJPlacement(string placementName) {
      System.Guid guid = System.Guid.NewGuid();
      string placementGuid = guid.ToString();

      _placementName = placementName;
      _guid = placementGuid;

      WeakReference placementRef = new WeakReference(this);
      placementDictionary.Add(placementGuid, placementRef);

      ApiBinding.Instance.CreatePlacement(placementGuid, placementName);
    }

    ~TJPlacement() {
      if (_guid != null) {
        TapjoyComponent.RemovePlacement(_guid);
      }
    }

    /**
     * @brief Create placement
     */
    public static TJPlacement CreatePlacement(string placementName) {
      return new TJPlacement(placementName);
    }

    /**
     * @brief Dismiss content which is showing
     */
    public static void DismissContent() {
      ApiBinding.Instance.DismissPlacementContent();
    }

    /**
     * @brief Request a content for the placement.
     *        This method should be called after SDK is connected.
     */
    public void RequestContent() {
      if (Tapjoy.IsConnected) {
        #if DEBUG
        Debug.Log("C#: Requesting content for placement " + _placementName + " guid:" + _guid);
        #endif
        ApiBinding.Instance.RequestPlacementContent(_guid);
      } else {
        Debug.Log("C#: Can not send placement becuause Tapjoy has not successfully connected.");
      }
    }

    internal static TapjoyAction<string> OnShowContentCalled;

    #if TEST
    public static void TriggerOnShowContentCalledForTesting(string placementName) {
      if (OnShowContentCalled != null) {
        OnShowContentCalled(placementName);
      }
    }
    #endif

    /**
     * @brief Shows a content for the placement.
     */
    public void ShowContent() {
      if (OnShowContentCalled != null) {
        OnShowContentCalled(_placementName);
      }

      ApiBinding.Instance.ShowPlacementContent(_guid);
    }

    // Getters

    /**
     * @brief Whether or not content for this placement has been returned and is ready
     *        to be presented
     *
     * @return if content is available and has a fill
     */
    public bool IsContentAvailable() {
      return ApiBinding.Instance.IsPlacementContentAvailable(_guid);
    }

    /**
     * @brief Whether or not the pre-loaded content for this placement has been cached
     *        and is ready to be presented
     *
     * @return if pre-loaded content is ready to be presented for this placement
     */
    public bool IsContentReady() {
      return ApiBinding.Instance.IsPlacementContentReady(_guid);
    }

    /**
     * @brief Returns the name of this placement.
     *
     * This is the same name passed to the
     * constructor when creating this #TapjoyUnity.TJPlacement object.
     *
     * @return the name for this placement
     */
    public string GetName() {
      return _placementName;
    }

    /**
     * @brief Sets the entry point for this placement instance.
     *
     * @param entryPoint 
     *        the entry point to set.
     */
    public void SetEntryPoint (TJEntryPoint entryPoint)
    {
      ApiBinding.Instance.SetEntryPoint (_guid, entryPoint);
    }

    /**
     * @brief Sets currency value for given currency ID
     *
     * @param currencyId
     *        currency id
     * @param balance
     *        balance amount     
     */
    public void SetCurrencyBalance (string currencyId, int balance) {
      ApiBinding.Instance.SetCurrencyBalance (_guid, currencyId, balance);
    }

    /**
     * @brief Gets currency balance for given currency ID
     *
     * @param currencyId
     *        currency id
     *
     * @return balance amount
     */
    public int GetCurrencyBalance (string currencyId) {
      return ApiBinding.Instance.GetCurrencyBalance (_guid, currencyId);
    }

    /**
     * @brief Sets currency required amount for given currency ID
     *
     * @param currencyId
     *        currency id
     * @param amount
     *        required amount
     */
    public void SetRequiredAmount (string currencyId, int amount) {
      ApiBinding.Instance.SetRequiredAmount (_guid, currencyId, amount);
    }

    /**
     * @brief Gets currency required amount for given currency ID
     *
     * @param currencyId
     *        currency id
     *
     * @return required amount
     */
    public int GetRequiredAmount (string currencyId) {
      return ApiBinding.Instance.GetRequiredAmount (_guid, currencyId);
    }

    #region Placement Delegates

    /**
     * @brief Delegate to be called when #RequestContent() is successful
     *        Check whether content is available and ready,
     *        using #IsContentAvailable and #IsContentReady
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnRequestSuccessHandler(TJPlacement placement);

    private static OnRequestSuccessHandler OnRequestSuccessInvoker;

    /**
     * @brief Event for #OnRequestSuccessHandler
     */
    public static event OnRequestSuccessHandler OnRequestSuccess {
      add {
        OnRequestSuccessInvoker += value;
      }
      remove {
        OnRequestSuccessInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when #RequestContent fails
     * @param placement
     *        the placement that was requested
     * @param error
     *        the error message
     */
    public delegate void OnRequestFailureHandler(TJPlacement placement, string error);

    private static OnRequestFailureHandler OnRequestFailureInvoker;

    /**
     * @brief Event for #OnRequestFailureHandler
     */
    public static event OnRequestFailureHandler OnRequestFailure {
      add {
        OnRequestFailureInvoker += value;
      }
      remove {
        OnRequestFailureInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when a content for the given placement is ready to show.
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnContentReadyHandler(TJPlacement placement);

    private static OnContentReadyHandler OnContentReadyInvoker;

    /**
     * @brief Event for #OnContentReadyHandler
     */
    public static event OnContentReadyHandler OnContentReady {
      add {
        OnContentReadyInvoker += value;
      }
      remove {
        OnContentReadyInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when a content for the given placement is showing.
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnContentShowHandler(TJPlacement placement);

    private static OnContentShowHandler OnContentShowInvoker;

    /**
     * @brief Event for #OnContentShowHandler
     */
    public static event OnContentShowHandler OnContentShow {
      add {
        OnContentShowInvoker += value;
      }
      remove {
        OnContentShowInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when a content for the given placement is dismissed.
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnContentDismissHandler(TJPlacement placement);

    private static OnContentDismissHandler OnContentDismissInvoker;

    /**
     * @brief Event for #OnContentDismissHandler
     */
    public static event OnContentDismissHandler OnContentDismiss {
      add {
        OnContentDismissInvoker += value;
      }
      remove {
        OnContentDismissInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when a click event has occurred.
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnClickHandler(TJPlacement placement);

    private static OnClickHandler OnClickInvoker;

    /**
     * @brief Event for #OnClickHandler
     */
    public static event OnClickHandler OnClick {
      add {
        OnClickInvoker += value;
      }
      remove {
        OnClickInvoker -= value;
      }
    }

    /**
     * @brief Called when a purchase has been requested.
     * @param placement
     *        the placement that was requested
     * @param request
     *        The request <br>
     *        Use #TapjoyUnity.TJActionRequest requestID to get the identifier of the campaign which make this request
     * @param productId
     *        the product identifier (SKU) of the in-app item to purchase
     */
    public delegate void OnPurchaseRequestHandler(TJPlacement placement, TJActionRequest request, string productId);

    private static OnPurchaseRequestHandler OnPurchaseRequestInvoker;

    /**
     * @brief Event for #OnPurchaseRequestHandler
     */
    public static event OnPurchaseRequestHandler OnPurchaseRequest {
      add {
        OnPurchaseRequestInvoker += value;
      }
      remove {
        OnPurchaseRequestInvoker -= value;
      }
    }

    /**
     * @brief Called when a reward unlock has been requested.
     * @param placement
     *        the placement that was requested
     * @param request
     *        The request <br>
     *        Use #TapjoyUnity.TJActionRequest requestID to get the unique identifier of this reward to prevent the reuse attack <br>
     *        Use #TapjoyUnity.TJActionRequest token to verify this reward request
     *
     * @param itemId
     *        the name of the rewarded item
     * @param quantity
     *        the quantity of the rewarded item
     */
    public delegate void OnRewardRequestHandler(TJPlacement placement, TJActionRequest request, string itemId, int quantity);

    private static OnRewardRequestHandler OnRewardRequestInvoker;

    /**
     * @brief Event for #OnRewardRequestHandler
     */
    public static event OnRewardRequestHandler OnRewardRequest {
      add {
        OnRewardRequestInvoker += value;
      }
      remove {
        OnRewardRequestInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when #SetCurrencyBalance is successful
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnSetCurrencyBalanceSuccessHandler (TJPlacement placement);

    private static OnSetCurrencyBalanceSuccessHandler OnSetCurrencyBalanceSuccessInvoker;

    /**
     * @brief Event for #OnSetCurrencyBalanceSuccessHandler
     */
    public static event OnSetCurrencyBalanceSuccessHandler OnSetCurrencyBalanceSuccess {
      add {
        OnSetCurrencyBalanceSuccessInvoker += value;
      }
      remove {
        OnSetCurrencyBalanceSuccessInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when #SetCurrencyBalance fails
     * @param placement
     *        the placement that was requested
     * @param code
     *        the error code
     * @param error
     *        the error message
     */
    public delegate void OnSetCurrencyBalanceFailureHandler (TJPlacement placement, int code, string error);

    private static OnSetCurrencyBalanceFailureHandler OnSetCurrencyBalanceFailureInvoker;

    /**
     * @brief Event for #OnSetCurrencyBalanceFailureHandler
     */
    public static event OnSetCurrencyBalanceFailureHandler OnSetCurrencyBalanceFailure {
      add {
        OnSetCurrencyBalanceFailureInvoker += value;
      }
      remove {
        OnSetCurrencyBalanceFailureInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when #SetRequiredAmount is successful
     * @param placement
     *        the placement that was requested
     */
    public delegate void OnSetCurrencyAmountRequiredSuccessHandler (TJPlacement placement);

    private static OnSetCurrencyAmountRequiredSuccessHandler OnSetCurrencyAmountRequiredSuccessInvoker;

    /**
     * @brief Event for #OnSetCurrencyAmountRequiredSuccessHandler
     */
    public static event OnSetCurrencyAmountRequiredSuccessHandler OnSetCurrencyAmountRequiredSuccess {
      add {
        OnSetCurrencyAmountRequiredSuccessInvoker += value;
      }
      remove {
        OnSetCurrencyAmountRequiredSuccessInvoker -= value;
      }
    }

    /**
     * @brief Delegate to be called when #SetRequiredAmount fails
     * @param placement
     *        the placement that was requested
     * @param code
     *        the error code
     * @param error
     *        the error message
     */
    public delegate void OnSetCurrencyAmountRequiredFailureHandler (TJPlacement placement, int code, string error);

    private static OnSetCurrencyAmountRequiredFailureHandler OnSetCurrencyAmountRequiredFailureInvoker;

    /**
     * @brief Event for #OnSetCurrencyAmountRequiredFailureHandler
     */
    public static event OnSetCurrencyAmountRequiredFailureHandler OnSetCurrencyAmountRequiredFailure {
      add {
        OnSetCurrencyAmountRequiredFailureInvoker += value;
      }
      remove {
        OnSetCurrencyAmountRequiredFailureInvoker -= value;
      }
    }

    #endregion

    #region Placement Bridge Calls (Native -> C#)

    internal static void DispatchPlacementEvent(string commaDelimitedMessage) {
      #if DEBUG
      Debug.Log("TapjoyUnity.DispatchPlacementEvent(" + commaDelimitedMessage + ")");
      #endif
      
      string[] args = commaDelimitedMessage.Split(',');
      string placementID = args[1];
      string placementName = Uri.UnescapeDataString(args[2]);

      // See if placement exists
      TJPlacement placement;
      WeakReference placementRef;

      if (placementDictionary.TryGetValue(placementID, out placementRef)) {
        placement = (TJPlacement) placementRef.Target;
        if (placement == null) {
          #if DEBUG
          Debug.LogWarning("PlacementEvent dispatched. But the placement was garbage-collected.");
          #endif
          placementDictionary.Remove(placementID);

          placement = new TJPlacement(placementName);
        }
      } else {
        placement = new TJPlacement(placementName);
      }

      // Switch through possible events
      switch (args[0]) {
      case "OnPlacementRequestSuccess":
        {
          if (OnRequestSuccessInvoker != null) {
            OnRequestSuccessInvoker(placement);
          }
          break;
        }
      case "OnPlacementRequestFailure":
         if (args.Length != 4) {
          return;
        }
        if (OnRequestFailureInvoker != null) {
          OnRequestFailureInvoker(placement, args[3]);
        }
        break;

      case "OnPlacementContentReady":
        if (OnContentReadyInvoker != null) {
          OnContentReadyInvoker(placement);
        }
        break;

      case "OnPlacementContentShow":
        if (OnContentShowInvoker != null) {
          OnContentShowInvoker(placement);
        }
        break;

      case "OnPlacementContentDismiss":
        if (OnContentDismissInvoker != null) {
          OnContentDismissInvoker(placement);
        }
        break;

      case "OnPlacementClick":
        if (OnClickInvoker != null) {
          OnClickInvoker(placement);
        }
        break;

      case "OnPurchaseRequest":
        if (args.Length != 6) {
          return;
        }

        if (OnPurchaseRequestInvoker != null) {
          string requestID = args[3];
          string token = args[4];
          string productID = args[5];
          OnPurchaseRequestInvoker(placement, new TJActionRequest(requestID, token), productID);
        }
        break;

      case "OnRewardRequest":
        if (args.Length != 7) {
          return;
        }

        if (OnRewardRequestInvoker != null) {
          string requestID = args[3];
          string token = args[4];
          string itemId = args[5];
          int quantity = int.Parse(args[6]);
          OnRewardRequestInvoker(placement, new TJActionRequest(requestID, token), itemId, quantity);
        }
        break;
      case "OnSetCurrencyBalanceFailure": {
          if (OnSetCurrencyBalanceFailureInvoker != null) {
            OnSetCurrencyBalanceFailureInvoker(placement, int.Parse(args[3]), args[4]);
          }
          break;
        }
      case "OnSetCurrencyBalanceSuccess":
        if (OnSetCurrencyBalanceSuccessInvoker != null) {
          OnSetCurrencyBalanceSuccessInvoker(placement);
        }
        break;
      case "OnSetCurrencyAmountRequiredFailure": {
          if (OnSetCurrencyAmountRequiredFailureInvoker != null) {
            OnSetCurrencyAmountRequiredFailureInvoker(placement, int.Parse(args[3]), args[4]);
          }
          break;
        }
      case "OnSetCurrencyAmountRequiredSuccess":
        if (OnSetCurrencyAmountRequiredSuccessInvoker != null) {
          OnSetCurrencyAmountRequiredSuccessInvoker(placement);
        }
        break;
      }
    }
    #endregion
  }
}
