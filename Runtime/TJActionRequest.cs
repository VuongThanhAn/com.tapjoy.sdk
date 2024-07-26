using UnityEngine;
using TapjoyUnity.Internal;

namespace TapjoyUnity {

  /**
   * @brief Request of an action to be performed.
   */
  public sealed class TJActionRequest {

    /**
     * @brief ID of this action request
     *
     * For OnPurchaseRequest, this is a campaign id.
     * For OnRewardRequest, this is the unique identifier of this reward to prevent the reuse attack.
     */
    public string requestID;

    /**
     * @brief the token to verify reward request
     */
    public string token;

    ~TJActionRequest() {
      if (requestID != null) {
        TapjoyComponent.RemoveActionRequest(requestID);
      }
    }

    internal TJActionRequest(string requestID, string token) {
      this.requestID = requestID;
      this.token = token;
    }

    /**
     * @brief Notify this action request is completed.
     */
    public void Completed() {
      ApiBinding.Instance.ActionRequestCompleted(requestID);
    }

    /**
     * @brief Notify this action request is cancelled.
     */
    public void Cancelled() {
      ApiBinding.Instance.ActionRequestCancelled(requestID);
    }
  }
}
