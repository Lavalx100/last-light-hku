using TMPro;
using UnityEngine;

/// <summary>
/// Handles in-game pause menu (ESC key)
/// Reuses ResultsUI panel but shows current time instead of final time
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuRoot; // The pause menu panel
    public TMP_Text currentTimeText; // Shows elapsed time
    
    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;
    
    // Public static property so other scripts (like FPSController) can check pause state
    public static bool IsPaused { get; private set; } = false;
    
    // Called by ResultsButtons to clean up static state before scene transitions
    public static void ResetPauseState()
    {
        IsPaused = false;
    }

    void Start()
    {
        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);
    }

    void Update()
    {
        // Don't allow pausing if results screen is already showing
        if (IsResultsScreenActive()) return;

        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        // Update time display while paused
        if (isPaused && currentTimeText != null && StopwatchManager.I != null)
        {
            currentTimeText.text = $"Time: {StopwatchManager.FormatTime(StopwatchManager.I.ElapsedSeconds)}";
        }
    }

    void Pause()
    {
        isPaused = true;
        IsPaused = true;

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(true);

        // Update current time
        if (currentTimeText != null && StopwatchManager.I != null)
            currentTimeText.text = $"Time: {StopwatchManager.FormatTime(StopwatchManager.I.ElapsedSeconds)}";

        // Pause game
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        IsPaused = false;

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool IsResultsScreenActive()
    {
        // Check if the finish trigger has activated the results screen
        var finishTrigger = FindObjectOfType<FinishTrigger>();
        if (finishTrigger != null)
        {
            // If results UI is active, don't allow pause menu
            var resultsUI = FindObjectOfType<ResultsUI>();
            if (resultsUI != null && resultsUI.gameObject.activeInHierarchy)
                return true;
        }
        return false;
    }
}
