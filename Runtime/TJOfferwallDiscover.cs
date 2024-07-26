using System;
using TapjoyUnity.Internal;

namespace TapjoyUnity
{
    public class TJOfferwallDiscover
    {
        public TJOfferwallDiscover()
        {
        }

        public static void RequestOfferwallDiscover(string placementName, float height = 262.0f)
        {
            ApiBinding.Instance.RequestOfferwallDiscover(placementName, height);
        }

        public static void RequestOfferwallDiscover(string placementName, float left, float top, float width, float height = 262.0f)
        {
            ApiBinding.Instance.RequestOfferwallDiscover(placementName, left, top, width, height);
        }

        public static void ShowOfferwallDiscover()
        {
            ApiBinding.Instance.ShowOfferwallDiscover();
        }

        public static void DestroyOfferwallDiscover()
        {
            ApiBinding.Instance.DestroyOfferwallDiscover();
        }

        internal static void DispatchOfferwallDiscoverEvent(string commaDelimitedMessage)
        {
#if DEBUG
            UnityEngine.Debug.Log("TapjoyUnity.DispatchOfferwallDiscoverEvent(" + commaDelimitedMessage + ")");
#endif

            string[] args = commaDelimitedMessage.Split(',');

            // Switch through possible events
            switch (args[0])
            {
                case "OnOfferwallDiscoverRequestSuccess":
                    {
                        if (OnRequestSuccessInvoker != null)
                        {
                            OnRequestSuccessInvoker();
                        }
                        break;
                    }
                case "OnOfferwallDiscoverRequestFailure":
                    {
                        if (OnRequestFailureInvoker != null)
                        {
                            OnRequestFailureInvoker(int.Parse(args[1]), args[2]);
                        }
                        break;
                    }
                case "OnOfferwallDiscoverContentReady":
                    {
                        if (OnContentReadyInvoker != null)
                        {
                            OnContentReadyInvoker();
                        }
                        break;
                    }
                case "OnOfferwallDiscoverContentError":
                    {
                        if (OnContentErrorInvoker != null)
                        {
                            OnContentErrorInvoker(int.Parse(args[1]), args[2]);
                        }
                        break;
                    }
                default:
                    break;
              
            }
        }

        /**
     * @brief Delegate to be called when #RequestContent() is successful
     *        the placement that was requested
     */
        public delegate void OnRequestSuccessHandler();

        private static OnRequestSuccessHandler OnRequestSuccessInvoker;

        /**
         * @brief Event for #OnRequestSuccessHandler
         */
        public static event OnRequestSuccessHandler OnRequestSuccess
        {
            add
            {
                OnRequestSuccessInvoker += value;
            }
            remove
            {
                OnRequestSuccessInvoker -= value;
            }
        }

        /**
         * @brief Delegate to be called when #RequestContent fails
         * @param error
         *        the error message
         */
        public delegate void OnRequestFailureHandler(int code, string error);

        private static OnRequestFailureHandler OnRequestFailureInvoker;

        /**
         * @brief Event for #OnRequestFailureHandler
         */
        public static event OnRequestFailureHandler OnRequestFailure
        {
            add
            {
                OnRequestFailureInvoker += value;
            }
            remove
            {
                OnRequestFailureInvoker -= value;
            }
        }

        /**
         * @brief Delegate to be called when a content is ready to show.
         */
        public delegate void OnContentReadyHandler();

        private static OnContentReadyHandler OnContentReadyInvoker;

        /**
         * @brief Event for #OnContentReadyHandler
         */
        public static event OnContentReadyHandler OnContentReady
        {
            add
            {
                OnContentReadyInvoker += value;
            }
            remove
            {
                OnContentReadyInvoker -= value;
            }
        }

        /**
         * @brief Delegate to be called when a content has a display error
         */
        public delegate void OnContentErrorHandler(int code, string error);

        private static OnContentErrorHandler OnContentErrorInvoker;

        /**
         * @brief Event for #OnContentErrorHandler
         */
        public static event OnContentErrorHandler OnContentError
        {
            add
            {
                OnContentErrorInvoker += value;
            }
            remove
            {
                OnContentErrorInvoker -= value;
            }
        }
    }
}
