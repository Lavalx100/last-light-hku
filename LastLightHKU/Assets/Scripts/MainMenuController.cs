using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu controller - handles mode selection and loading gameplay scene
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "SampleScene"; // Change to your gameplay scene name

    [Header("UI")]
    [SerializeField] private TMP_Text bestTimeText;

    // Keys for saving best times per mode
    private const string BestScaryKey = "BestTimeSeconds_Scary";
    private const string BestEasyKey = "BestTimeSeconds_Easy";

    void Start()
    {
        // Load any previously saved mode preference
        GameModeStore.LoadMode();
        
        // Display best times
        RefreshBestTimeUI();
    }

    /// <summary>
    /// Called by Night button
    /// </summary>
    public void PlayNight()
    {
        GameModeStore.SetMode(GameMode.Scary);
        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Called by Day button
    /// </summary>
    public void PlayDay()
    {
        GameModeStore.SetMode(GameMode.Easy);
        SceneManager.LoadScene(gameplaySceneName);
    }

    void RefreshBestTimeUI()
    {
        float bestScary = PlayerPrefs.GetFloat(BestScaryKey, -1f);
        float bestEasy = PlayerPrefs.GetFloat(BestEasyKey, -1f);

        string scaryStr = bestScary < 0 ? "Night: --:--.---" : "Night: " + FormatTime(bestScary);
        string easyStr = bestEasy < 0 ? "Day: --:--.---" : "Day: " + FormatTime(bestEasy);

        if (bestTimeText != null)
            bestTimeText.text = $"Best Personal Time\n{scaryStr}\n{easyStr}";
    }

    static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        float rem = seconds - minutes * 60f;
        int sec = Mathf.FloorToInt(rem);
        int ms = Mathf.FloorToInt((rem - sec) * 1000f);

        return $"{minutes:00}:{sec:00}.{ms:000}";
    }

    [ContextMenu("Clear Best Time Record")]
    public void ClearBestTimeRecord()
    {
        PlayerPrefs.DeleteKey(BestScaryKey);
        PlayerPrefs.DeleteKey(BestEasyKey);
        PlayerPrefs.Save();
        RefreshBestTimeUI();
        Debug.Log("[MainMenuController] Best time records cleared.");
    }
}
