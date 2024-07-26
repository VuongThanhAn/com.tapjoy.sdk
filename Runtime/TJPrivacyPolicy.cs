using UnityEngine;
using TapjoyUnity.Internal;
using System;

namespace TapjoyUnity {

  public sealed class TJPrivacyPolicy {

    private TJPrivacyPolicy() {
      ApiBinding.Instance.GetPrivacyPolicy();
    }

    /**
     * @brief Returns the TJPrivacyPolicy instance for calling methods to set GDPR, User's consent, below consent age ,and US Privacy policy flags
     */
    public static TJPrivacyPolicy GetPrivacyPolicy() {
        return new TJPrivacyPolicy();
    }

    /** 
     * @brief This can be used by the integrating App to indicate if the user falls in any of the GDPR applicable countries
     * (European Economic Area). The value should be set to YES when User (Subject) is applicable to GDPR regulations
     * and NO when User is not applicable to GDPR regulations. In the absence of this call, Tapjoy server makes the
     * determination of GDPR applicability.
     *
     * @param gdprApplicable 
     *        YES if GDPR applies to this user, NO otherwise
     */
    public void SetSubjectToGDPR(TJStatus gdprApplicable) {
        ApiBinding.Instance.SetSubjectToGDPR(gdprApplicable);
    }

    /**
     * @brief Returns configured GDPR value.
     * The value should be returned to YES when User (Subject) is applicable to GDPR regulations
     * and NO when User is not applicable to GDPR regulations.
     *
     * @return YES if GDPR applies to this user, NO otherwise
     */
    public TJStatus GetSubjectToGDPR() {
        return (TJStatus)ApiBinding.Instance.GetSubjectToGDPR();
    }

    /**
     * @brief This is used for sending User's consent to behavioral advertising such as in the context of GDPR
     * The consent value can be YES (User has not provided consent), NO (User has provided consent) or a daisybit string as suggested in IAB's Transparency and Consent Framework
     * @param consent 
     *        The user consent value
     */
    public void SetUserConsent(TJStatus consent) {
        ApiBinding.Instance.SetUserConsent(consent);
    }

    /**
     * @brief Returns user's consent to behavioral advertising such as in the context of GDPR
     * The consent value can be NO (User has not provided consent), YES (User has provided consent) or a daisybit string as suggested in IAB's Transparency and Consent Framework
     * @return The user consent value
     */
    public TJStatus GetUserConsent()
    {
        return (TJStatus)ApiBinding.Instance.GetUserConsent();
    }

    /**
     * @brief In the US, the Children’s Online Privacy Protection Act (COPPA) imposes certain requirements on operators of online services that (a) have actual knowledge that the connected
     * user is a child under 13 years of age, or (b) operate services (including apps) that are directed to children under 13.
     *
     * Similarly, the GDPR imposes certain requirements in connection with data subjects who are below the applicable local minimum age for online consent (ranging from 13 to 16,
     * as established by each member state).
     *
     * For applications that are not directed towards children under 13 years of age, but still have a minority share of users known to be under the applicable minimum age,
     * utilize this method to access Tapjoy’s monetization capability. This method will set ad_tracking_enabled to false for Tapjoy which only shows the user contextual ads. No ad tracking will be done on this user.
     *
     * @param belowConsentAge YES if below consent age (COPPA) applies to this user, NO otherwise
     */
    public void SetBelowConsentAge (TJStatus belowConsentAge)
    {
      ApiBinding.Instance.SetBelowConsentAge(belowConsentAge);
    }

    /**
     * @brief Returns the consent age (COPPA) flag applied to the user.
     *
     * @return YES if below consent age (COPPA) applies to this user, NO otherwise
     */
    public TJStatus GetBelowConsentAge ()
    {
        return (TJStatus)ApiBinding.Instance.GetBelowConsentAge();
    }

    /**
    * @brief This is used for sending US Privacy value to behavioral advertising such as in the context of GDPR
    * The value can be in IAB's US Privacy String Format consists of specification version to encode the string in number, explicit notice or opportunity to opt out in enum, opt-out sale in enum, LSPA covered transaction in enum .
    * eg. "1YNN" where 1 is char in string for the version, Y = YES, N = No, - = Not Applicable
    * See: IAB suggested US Privacy String Format : https://github.com/InteractiveAdvertisingBureau/USPrivacy/blob/master/CCPA/Version%201.0/US%20Privacy%20String.md#us-privacy-string-format
    *
    * @param privacyPolicy 
    *        The us privacy value string
    */
    public void SetUSPrivacy (string privacyPolicy)
    {
      ApiBinding.Instance.SetUSPrivacy (privacyPolicy);
    }

    /**
    * @brief Returns US Privacy value to behavioral advertising such as in the context of GDPR
    *
    * @return The us privacy value string
    */
    public string GetUSPrivacy ()
    {
      return ApiBinding.Instance.GetUSPrivacy ();
    }
    
  }
}
