using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TapjoyUnity;

public class OfferwallDiscoverScreen : MonoBehaviour
{
    public TMP_Text statusMessageText;

    public TMP_InputField owdPlacementInput;
    public TMP_InputField leftInput;
    public TMP_InputField topInput;
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;

    public Button requestOwdButton;
    public Button clearOwdButton;


    private void Awake() { 
        widthInput.text = System.Math.Floor(Screen.width / Tapjoy.GetScreenScale()).ToString();
    }

    void OnEnable()
    {
        TJOfferwallDiscover.OnRequestSuccess += OnRequestSuccess;
        TJOfferwallDiscover.OnRequestFailure += OnRequestFailure;
        TJOfferwallDiscover.OnContentReady += OnContentReady;
        TJOfferwallDiscover.OnContentError += OnContentError;
    }

    void OnDisable()
    {
        TJOfferwallDiscover.OnRequestSuccess -= OnRequestSuccess;
        TJOfferwallDiscover.OnRequestFailure -= OnRequestFailure;
        TJOfferwallDiscover.OnContentReady -= OnContentReady;
        TJOfferwallDiscover.OnContentError -= OnContentError;
    }

    private void OnRequestSuccess()
    {
        SetStatusMessage("RequestSuccess");
    }

    private void OnRequestFailure(int code, string error)
    {
        SetStatusMessage("RequestFailure: " + code + " - " + error);
    }

    private void OnContentReady()
    {
        SetStatusMessage("Content is ready");
        TJOfferwallDiscover.ShowOfferwallDiscover();
    }

    private void OnContentError(int code, string error)
    {
        SetStatusMessage("ContentError: " + code + " - " + error);
    }

    public void RequestOwdContent()
    {
        var positionValuesReady = true;

        positionValuesReady &= float.TryParse(leftInput.text, out var outLeft);
        positionValuesReady &= float.TryParse(topInput.text, out var outTop);
        positionValuesReady &= float.TryParse(widthInput.text, out var outWidth);
        positionValuesReady &= float.TryParse(heightInput.text, out var outHeight);

        if (positionValuesReady)
        {
            TJOfferwallDiscover.RequestOfferwallDiscover(owdPlacementInput.text, outLeft, outTop, outWidth, outHeight);
        }
        else
        {
            SetStatusMessage("Please set the valid position and size value");
        }
    }

    public void ClearOwdContent()
    {
        TJOfferwallDiscover.DestroyOfferwallDiscover();
        SetStatusMessage("Content cleared");
    }
    
    private void SetStatusMessage(string message)
    {
        Debug.Log(message);
        statusMessageText.text = message;
    }
}
