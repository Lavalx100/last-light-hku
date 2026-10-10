using TMPro;
using UnityEngine;

public class StopwatchUI : MonoBehaviour
{
    public TMP_Text timerText;

    void OnEnable()
    {
        if (StopwatchManager.I != null)
            StopwatchManager.I.OnTimeChanged += Handle;
    }

    void OnDisable()
    {
        if (StopwatchManager.I != null)
            StopwatchManager.I.OnTimeChanged -= Handle;
    }

    void Start()
    {
        if (timerText == null) timerText = GetComponent<TMP_Text>();
        Handle(StopwatchManager.I != null ? StopwatchManager.I.ElapsedSeconds : 0f);
    }

    void Handle(float seconds)
    {
        if (timerText == null) return;
        timerText.text = StopwatchManager.FormatTime(seconds);
    }
}
