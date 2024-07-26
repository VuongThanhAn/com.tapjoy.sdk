using UnityEngine;
using UnityEngine.UI;

public class ScreenOrientationDetector : MonoBehaviour
{
    private HorizontalLayoutGroup horizontalLayoutGroup;
    private ScreenOrientation currentOrientation;
    private int landscapeLeftPadding = 100;
    private int portraitLeftPadding = 10;


    void Start()
    {
        currentOrientation = Screen.orientation;
        Debug.Log("Current Orientation: " + currentOrientation);

        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        UpdatePadding();
    }

    void Update()
    {

        if (currentOrientation != Screen.orientation)
        {
            currentOrientation = Screen.orientation;
            UpdatePadding();
        }
    }

    void UpdatePadding()
    {
        Debug.Log("Screen orientation changed to: " + currentOrientation);

        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            horizontalLayoutGroup.padding.left = portraitLeftPadding;
            horizontalLayoutGroup.padding.right = portraitLeftPadding;
        }
        else
        {
            horizontalLayoutGroup.padding.left = landscapeLeftPadding;
            horizontalLayoutGroup.padding.right = landscapeLeftPadding;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroup.GetComponent<RectTransform>());
    }
}