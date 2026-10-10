using UnityEngine;
using UnityEngine.UI;

public class PhoneTabs : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button tabTasks;
    public Button tabHints;
    public Button tabRelics;
    public Button tabBattery;

    [Header("Pages")]
    public GameObject pageTasks;
    public GameObject pageHints;
    public GameObject pageRelics;
    public GameObject pageBattery;

    void Awake()
    {
        if (tabTasks != null) tabTasks.onClick.AddListener(() => Show(pageTasks));
        if (tabHints != null) tabHints.onClick.AddListener(() => Show(pageHints));
        if (tabRelics != null) tabRelics.onClick.AddListener(() => Show(pageRelics));
        if (tabBattery != null) tabBattery.onClick.AddListener(() => Show(pageBattery));
    }

    void Start()
    {
        Show(pageBattery); // start on Battery since that's what you're currently showing
    }

    public void Show(GameObject pageToShow)
    {
        if (pageTasks != null) pageTasks.SetActive(pageToShow == pageTasks);
        if (pageHints != null) pageHints.SetActive(pageToShow == pageHints);
        if (pageRelics != null) pageRelics.SetActive(pageToShow == pageRelics);
        if (pageBattery != null) pageBattery.SetActive(pageToShow == pageBattery);
    }
}
