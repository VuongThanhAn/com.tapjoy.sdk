using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TapjoyUnity;
using TMPro;
using System.Text.RegularExpressions;
#if UNITY_IOS_AD_SUPPORT
#if UNITY_IOS
	using Unity.Advertisement.IosSupport;
#endif
#endif

public class MainScreen : MonoBehaviour
{
    string sdkKey = "";

    public TMP_Text statusMessageText;

    public Button connectButton;
    public Button showOfferwallButton;

    public Button getCurrencyButton;
    public Button spendCurrencyButton;
    public Button awardCurrencyButton;

    public TMP_InputField examplePlacementInput;

    public Button requestButton;
    public Button showButton;

    public TMP_Dropdown entryPointDropdown;

    public TMP_InputField currencyIdInput;
    public TMP_InputField currencyBalanceInput;
    public TMP_InputField currencyRequiredAmountInput;


    public Button purchaseButton;

    public TMP_Text versionLabel;
    public Button supportWebPageButton;

    TJPlacement offerwallPlacement;
    TJPlacement examplePlacement;

    TJPlacement.OnContentReadyHandler readyHandler;
    TJPlacement.OnContentDismissHandler dismissHandler;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS
#if UNITY_IOS_AD_SUPPORT
        //Show ATT before connection, Only applicable to IOS
		if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == 
		ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
		{

			ATTrackingStatusBinding.RequestAuthorizationTracking();

		}
#endif
        // This sdk key will be only used when the auto connect mode disabled in Tapjoy window.
        sdkKey = "E7CuaoUWRAWdz_5OUmSGsgEBXHdOwPa8de7p4aseeYP01mecluf-GfNgtXlF";
#elif UNITY_ANDROID
        sdkKey = "u6SfEbh_TA-WMiGqgQ3W8QECyiQIURFEeKm0zbOggubusy-o5ZfXp33sTXaD";
#endif

        //init state of the components
        examplePlacementInput.text = "offerwall_unit";
        showButton.interactable = false;
        versionLabel.text = $"SDK Version : {Tapjoy.Version}";

        // Connect delegates
        Tapjoy.OnConnectSuccess += HandleConnectSuccess;
        Tapjoy.OnConnectFailed += HandleConnectFailed;
        Tapjoy.OnConnectWarning += HandleConnectWarning;

        // Placement delegates
        TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
        TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
        TJPlacement.OnContentShow += HandlePlacementContentShow;
        TJPlacement.OnClick += HandlePlacementOnClick;
        TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
        TJPlacement.OnRewardRequest += HandleOnRewardRequest;
        TJPlacement.OnSetCurrencyBalanceSuccess += HandleSetCurrencyBalanceSuccess;
        TJPlacement.OnSetCurrencyBalanceFailure += HandleSetCurrencyBalanceFailure;
        TJPlacement.OnSetCurrencyAmountRequiredSuccess += HandleSetRequiredAmountSuccess;
        TJPlacement.OnSetCurrencyAmountRequiredFailure += HandleSetRequiredAmountFailure;

        // Currency delegates
        Tapjoy.OnAwardCurrencyResponse += HandleAwardCurrencyResponse;
        Tapjoy.OnAwardCurrencyResponseFailure += HandleAwardCurrencyResponseFailure;
        Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
        Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;
        Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
        Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
        Tapjoy.OnEarnedCurrency += HandleEarnedCurrency;
    }

    void Update()
    {
        // Quit app on BACK key.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Request connect if auto connect is not checked in the Unity Tapjoy editor.
    public void RequestConnect()
    {
        Debug.Log($"SDK Key:{sdkKey}");
        Dictionary<string, object> connectFlags = new Dictionary<string, object>();
        connectFlags.Add("TJC_OPTION_ENABLE_LOGGING", "true");

        Tapjoy.Connect(sdkKey, connectFlags);
    }

    public void RequestOfferwall()
    {
        SetStatusMessage("Requesting offerwall...");

        offerwallPlacement = TJPlacement.CreatePlacement("offerwall_unit");

        readyHandler = (TJPlacement placement) =>
        {
            SetStatusMessage("Offerwall content is ready.");

            offerwallPlacement.ShowContent();
            TJPlacement.OnContentReady -= readyHandler;
        };
        TJPlacement.OnContentReady += readyHandler;

        dismissHandler = (TJPlacement placement) =>
        {
            SetStatusMessage($"Offerwall has been dismissed.");
            TJPlacement.OnContentDismiss -= dismissHandler;
        };
        TJPlacement.OnContentDismiss += dismissHandler;

        offerwallPlacement.RequestContent();
    }

    public void GetCurrency()
    {
        SetStatusMessage("Getting Currency...");
        Tapjoy.GetCurrencyBalance();
    }

    public void SpendCurrency()
    {
        SetStatusMessage("Spending Currency...");
        Tapjoy.SpendCurrency(10);
    }

    public void AwardCurrency()
    {
        SetStatusMessage("Awarding Currency...");
        Tapjoy.AwardCurrency(10);
    }

    public void RequestExampleContent()
    {
        SetStatusMessage("Requesting example content...");

        examplePlacement = TJPlacement.CreatePlacement(examplePlacementInput.text);
        readyHandler = (TJPlacement placement) =>
        {
            SetStatusMessage($"TJPlacement {placement.GetName()} content is ready.");
            showButton.interactable = true;
            TJPlacement.OnContentReady -= readyHandler;
        };
        TJPlacement.OnContentReady += readyHandler;

        dismissHandler = (TJPlacement placement) =>
        {
            SetStatusMessage($"TJPlacement {placement.GetName()} has been dismissed.");
            showButton.interactable = false;
            TJPlacement.OnContentDismiss -= dismissHandler;
        };
        TJPlacement.OnContentDismiss += dismissHandler;

        examplePlacement.SetEntryPoint((TJEntryPoint)entryPointDropdown.value);

        if (currencyIdInput.text != "")
        {
            if (currencyBalanceInput.text != "" && currencyBalanceInput.text == Regex.Replace(currencyBalanceInput.text, @"[^0-9]", ""))
            {
                examplePlacement.SetCurrencyBalance(currencyBalanceInput.text, int.Parse(currencyBalanceInput.text));
            }

            if (currencyRequiredAmountInput.text != "" && currencyRequiredAmountInput.text == Regex.Replace(currencyRequiredAmountInput.text, @"[^0-9]", ""))
            {
                examplePlacement.SetRequiredAmount(currencyBalanceInput.text, int.Parse(currencyRequiredAmountInput.text));
            }
        }

        examplePlacement.RequestContent();
    }

    public void ShowExampleContent()
    {
        SetStatusMessage("Showing example content..");
        examplePlacement.ShowContent();
    }

    public void Purchase()
    {
        Tapjoy.TrackPurchase("USD", 0.99);
        SetStatusMessage("Sent track purchase");
    }

    public void OpenSupportPage()
    {
        Application.OpenURL(Tapjoy.GetSupportURL());
    }

    private string getDummySkuDetails()
    {
        return "{\"title\":\"TITLE\",\"price\":\"$3.33\",\"type\":\"inapp\",\"description\":\"DESC\",\"price_amount_micros\":3330000,\"price_currency_code\":\"USD\",\"productId\":\"3\"}";
    }

    void SetStatusMessage(string message)
    {
        Debug.Log(message);
        statusMessageText.text = message;
    }

    #region Tapjoy Delegate Handlers

    #region Connect Delegate Handlers

    void HandleConnectSuccess()
    {
        SetStatusMessage("Tapjoy Connected");
        connectButton.gameObject.SetActive(false);
    }

    void HandleConnectFailed(int code, string message)
    {
        SetStatusMessage($"Tapjoy SDK failed to connect. Code: {code} Message: {message}");
    }

    void HandleConnectWarning(int code, string message)
    {
        SetStatusMessage($"Tapjoy SDK connect succeeded with warning Code: {code} Message: {message}");
    }

    #endregion

    #region Placement Delegate Handlers

    public void HandlePlacementRequestSuccess(TJPlacement placement)
    {
        if (placement.IsContentAvailable())
        {
            SetStatusMessage($"Content available for {placement.GetName()}");
        }
        else
        {
            SetStatusMessage($"No content available for {placement.GetName()}");
        }
    }

    void HandlePlacementRequestFailure(TJPlacement placement, string error)
    {
        SetStatusMessage($"Request for {placement.GetName()} has failed because: {error}");
    }

    void HandlePlacementContentShow(TJPlacement placement)
    {
        SetStatusMessage("Handle placement content show");
    }

    void HandlePlacementOnClick(TJPlacement placement)
    {
        SetStatusMessage("HandlePlacementOnClick");
    }

    void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
    {
        SetStatusMessage("HandleOnPurchaseRequest");
        request.Completed();
    }

    void HandleOnRewardRequest(TJPlacement placement, TJActionRequest request, string itemId, int quantity)
    {
        SetStatusMessage("HandleOnRewardRequest");
        request.Completed();
    }

    void HandleSetCurrencyBalanceSuccess(TJPlacement placement)
    {
        SetStatusMessage($"SetCurrencyBalance for {placement.GetName()} has succeeded. Balance: {placement.GetCurrencyBalance(currencyIdInput.text)}");
    }

    void HandleSetCurrencyBalanceFailure(TJPlacement placement, int code, string error)
    {
        SetStatusMessage($"SetCurrencyBalance for {placement.GetName()} has failed because: {error}(code: {code})");
    }

    void HandleSetRequiredAmountSuccess(TJPlacement placement)
    {
        SetStatusMessage($"SetRequiredAmount for {placement.GetName()} has succeeded. Balance: {placement.GetRequiredAmount(currencyIdInput.text)}");
    }

    void HandleSetRequiredAmountFailure(TJPlacement placement, int code, string error)
    {
        SetStatusMessage($"SetRequiredAmount for {placement.GetName()} has failed because: {error}(code: {code})");
    }

    #endregion

    #region Currency Delegate Handlers

    void HandleAwardCurrencyResponse(string currencyName, int balance)
    {
        SetStatusMessage($"Awarded currency -- {currencyName} balance: {balance}");
    }

    void HandleAwardCurrencyResponseFailure(string error)
    {
        SetStatusMessage($"AwardCurrency failed: {error}");
    }

    void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
    {
        SetStatusMessage($"{currencyName} balance: {balance}");
    }

    void HandleGetCurrencyBalanceResponseFailure(string error)
    {
        SetStatusMessage($"GetCurrencyBalance failed: {error}");
    }

    void HandleSpendCurrencyResponse(string currencyName, int balance)
    {
        SetStatusMessage($"{currencyName} balance: {balance}");
    }

    void HandleSpendCurrencyResponseFailure(string error)
    {
        SetStatusMessage($"SpendCurrency failed: {error}");
    }

    void HandleEarnedCurrency(string currencyName, int amount)
    {
        SetStatusMessage($"{currencyName} earned: {amount}");

        Tapjoy.ShowDefaultEarnedCurrencyAlert();
    }

    #endregion

    #endregion
}