using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public GameObject resultsUIRoot; // drag your ResultsCanvas root
    public bool requireAllTasks = true;

    bool finished;

    void Start()
    {
        if (resultsUIRoot != null)
            resultsUIRoot.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        if (requireAllTasks && TaskManager.I != null && !TaskManager.I.AreAllTasksComplete())
            return;

        finished = true;

        if (StopwatchManager.I != null)
            StopwatchManager.I.FinishRun();

        if (resultsUIRoot != null)
            resultsUIRoot.SetActive(true);

        // Optional: freeze player controls
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[FinishTrigger] Game finished.");
    }
}
