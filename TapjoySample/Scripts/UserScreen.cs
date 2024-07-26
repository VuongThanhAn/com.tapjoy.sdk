using TapjoyUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserScreen : MonoBehaviour
{
    public TMP_Text statusMessageText;
    public TMP_InputField userIdInput;
    public TMP_InputField levelInput;
    public TMP_InputField maxLevelInput;
    public TMP_Dropdown userSegmentDropdown;

    public Button setSegmentButton;
    public Button clearSegmentButton;

    public TMP_InputField userTagInput;
    public Button addTagButton;
    public Button removeTagButton;
    public Button clearTagButton;
    public TMP_Text currentTagsText;

    public TMP_Dropdown belowConsentAgeDropdown;
    public TMP_Dropdown subjectToGDPRDropdown;
    public TMP_Dropdown userConsentDropdown;

    public TMP_InputField usPrivacyInput;
    public Button usPrivacySetButton;
    private string currentUserTags = "";

    void OnEnable()
    {
        Debug.Log("C# UserScreen Enable -- Adding Tapjoy User ID delegates");

        Tapjoy.OnSetUserIDSuccess += HandleSetUserIDSuccess;
        Tapjoy.OnSetUserIDFailed += HandleSetUserIDFailed;

        InitValues();
    }

    void OnDisable()
    {
        Debug.Log("C#: UserScreen -- Disabling and removing Tapjoy User ID Delegates");

        // Placement delegates
        Tapjoy.OnSetUserIDSuccess -= HandleSetUserIDSuccess;
        Tapjoy.OnSetUserIDFailed -= HandleSetUserIDFailed;
    }

    #region SetUserID Handler
    private void HandleSetUserIDSuccess()
    {
        SetStatusMessage("HandleSetUserIDSuccess");
    }

    private void HandleSetUserIDFailed(int code, string error)
    {
        SetStatusMessage($"HandleSetUserIDFailed: {error} Code: {code}");
    }
    #endregion
    
    private void InitValues()
    {
        if (Tapjoy.IsConnected)
        {
            userIdInput.text = Tapjoy.GetUserID();
            levelInput.text = Tapjoy.GetUserLevel().ToString();
            maxLevelInput.text = Tapjoy.GetMaxLevel().ToString();
            userSegmentDropdown.value = (int)Tapjoy.GetUserSegment() == -1 ? 3 : (int)Tapjoy.GetUserSegment();

            UpdateUserTags();

            belowConsentAgeDropdown.value = (int)TJPrivacyPolicy.GetPrivacyPolicy().GetBelowConsentAge();
            subjectToGDPRDropdown.value = (int)TJPrivacyPolicy.GetPrivacyPolicy().GetSubjectToGDPR();
            userConsentDropdown.value = (int)TJPrivacyPolicy.GetPrivacyPolicy().GetUserConsent();
            usPrivacyInput.text = TJPrivacyPolicy.GetPrivacyPolicy().GetUSPrivacy();
            
            Debug.Log($"belowConsentAge={belowConsentAgeDropdown.value}, gdpr={subjectToGDPRDropdown.value}, userConsent={userConsentDropdown.value}, usPrivacy={usPrivacyInput.text}");
        }
    }
    
    public void SetUserProperties()
    {
        Tapjoy.SetUserID(userIdInput.text);

        if (int.TryParse(levelInput.text, out var temp))
        {
            Tapjoy.SetUserLevel(temp);
        } else
        {
            Tapjoy.SetUserLevel(-1);
        }

        if (int.TryParse(maxLevelInput.text, out temp))
        {
            Tapjoy.SetMaxLevel(temp);
        } else {
            Tapjoy.SetMaxLevel(-1);
        }

        Tapjoy.SetUserSegment(userSegmentDropdown.value == 3 ? (TJSegment)(-1) : (TJSegment)userSegmentDropdown.value);
        
        SetStatusMessage("Saved user properties");
    }

    public void ClearUserProperties()
    {
        Tapjoy.SetUserLevel(-1);
        Tapjoy.SetMaxLevel(-1);
        userIdInput.text = "";
        levelInput.text = "";
        maxLevelInput.text = "";
    }

    private void UpdateUserTags()
    {
        //Clear existing string
        currentUserTags = "";
        foreach (var userTag in Tapjoy.GetUserTags())
        {
            if (currentUserTags != "") currentUserTags += ", ";
            currentUserTags += userTag;
        }

        if (currentUserTags != "")
        {
            currentTagsText.text = "Tags: " + currentUserTags;
        }
        else
        {
            currentTagsText.text = "";
        }
    }

    public void AddUserTag()
    {
        Tapjoy.AddUserTag(userTagInput.text);
        SetStatusMessage("Added user tag: " + userTagInput.text);
        
        userTagInput.text = "";
        UpdateUserTags();
    }

    public void RemoveUserTag()
    {
        Tapjoy.RemoveUserTag(userTagInput.text);
        SetStatusMessage("Removed user tag: " + userTagInput.text);

        userTagInput.text = "";
        UpdateUserTags();
    }

    public void ClearUserTag()
    {
        Tapjoy.ClearUserTags();
        userTagInput.text = "";
        UpdateUserTags();
        SetStatusMessage("Cleared user tags");
    }
    
    public void OnBelowConsentAgeDropdownValueChange()
    {
        TJPrivacyPolicy.GetPrivacyPolicy().SetBelowConsentAge((TJStatus)belowConsentAgeDropdown.value);
    }
    
    public void OnSubjectToGDPRDropdownValueChange()
    {
        TJPrivacyPolicy.GetPrivacyPolicy().SetSubjectToGDPR((TJStatus)subjectToGDPRDropdown.value);
    }

    public void OnUserConsentDropdownValueChange()
    {
        TJPrivacyPolicy.GetPrivacyPolicy().SetUserConsent((TJStatus)userConsentDropdown.value);

    }

    public void SetUsPrivacy()
    {
        TJPrivacyPolicy.GetPrivacyPolicy().SetUSPrivacy(usPrivacyInput.text);
        SetStatusMessage("Saved US privacy value: " + usPrivacyInput.text);
    }

    private void SetStatusMessage(string message)
    {
        Debug.Log(message);
        statusMessageText.text = message;
    }
}