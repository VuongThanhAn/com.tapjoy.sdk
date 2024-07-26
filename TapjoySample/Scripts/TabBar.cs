using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TabBar : MonoBehaviour
{
    public Button mainButton;
    public Button userButton;
    public Button owdButton;

    public Image indicator0;
    public Image indicator1;
    public Image indicator2;

    public GameObject mainScreen;
    public GameObject userScreen;
    public GameObject owdScreen;

    Color selectedColor = new Color(1, 0.5f, 0);

    readonly ArrayList indicators = new ArrayList();
    readonly ArrayList screens = new ArrayList();


    // Start is called before the first frame update
    void Start()
    {
        indicators.Add(indicator0);
        indicators.Add(indicator1);
        indicators.Add(indicator2);

        screens.Add(mainScreen);
        screens.Add(userScreen);
        screens.Add(owdScreen);

        mainButton.onClick.AddListener(() => { this.SelectTab(0); });

        userButton.onClick.AddListener(() => { this.SelectTab(1); });

        owdButton.onClick.AddListener(() => { this.SelectTab(2); });

        this.SelectTab(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    internal void SelectTab(int index)
    {
        for (var i = 0; i < indicators.Count; i++)
        {
            ((Image)indicators[i]).color = i == index ? selectedColor : Color.white;
        }

        for (var i = 0; i < screens.Count; i++)
        {
            ((GameObject)screens[i]).SetActive(i == index);
        }
    }
}