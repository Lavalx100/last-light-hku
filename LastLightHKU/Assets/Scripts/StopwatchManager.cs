using System;
using UnityEngine;

public class StopwatchManager : MonoBehaviour
{
    public static StopwatchManager I { get; private set; }

    public event Action<float> OnTimeChanged;   // seconds
    public event Action<float> OnRunFinished;   // final seconds

    public bool IsRunning { get; private set; }
    public float ElapsedSeconds { get; private set; }

    float lastSent;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    void Start()
    {
        StartRun();
    }

    void Update()
    {
        if (!IsRunning) return;

        ElapsedSeconds += Time.deltaTime;

        // update UI at ~10Hz so it is stable and cheap
        if (ElapsedSeconds - lastSent >= 0.1f)
        {
            lastSent = ElapsedSeconds;
            OnTimeChanged?.Invoke(ElapsedSeconds);
        }
    }

    public void StartRun()
    {
        ElapsedSeconds = 0f;
        lastSent = -999f;
        IsRunning = true;
        OnTimeChanged?.Invoke(ElapsedSeconds);
    }

    public void FinishRun()
    {
        if (!IsRunning) return;

        IsRunning = false;
        OnTimeChanged?.Invoke(ElapsedSeconds);
        OnRunFinished?.Invoke(ElapsedSeconds);
    }

    public static string FormatTime(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        int total = Mathf.FloorToInt(seconds);
        int mins = total / 60;
        int secs = total % 60;
        int ms = Mathf.FloorToInt((seconds - total) * 1000f);
        return $"{mins:00}:{secs:00}.{ms:000}";
    }

    [ContextMenu("Clear Best Time Record")]
    public void ClearBestTimeRecord()
    {
        PlayerPrefs.DeleteKey("BEST_TIME_SECONDS");
        PlayerPrefs.Save();
        Debug.Log("[StopwatchManager] Best time record cleared.");
    }
}
