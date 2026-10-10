using System;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_SignalCalibration : MonoBehaviour, IMinigame
{
    [Header("UI on the arcade screen")]
    public GameObject gamePanel;
    public RectTransform fillRect;   // ProgressBarFill RectTransform
    public Text statusText;

    [Header("Tuning")]
    public float progressPerTap = 0.08f;
    public float decayPerSecond = 0.02f;
    public KeyCode tapKey = KeyCode.Space;

    private Action _onWin;
    private bool _running;
    private float _p;

    private float _fullWidth;

    private void Start()
    {
        if (gamePanel != null) gamePanel.SetActive(false);

        if (fillRect != null)
            _fullWidth = fillRect.sizeDelta.x; // assumes fill is full width in editor
    }

    public void Begin(Action onWin)
    {
        _onWin = onWin;
        _running = true;
        _p = 0f;

        if (gamePanel != null) gamePanel.SetActive(true);
        RefreshUI();
    }

    private void Update()
    {
        if (!_running) return;

        if (Input.GetKeyDown(tapKey))
        {
            _p += progressPerTap;
            if (_p >= 1f)
            {
                _p = 1f;
                RefreshUI();
                EndWin();
                return;
            }
        }

        if (decayPerSecond > 0f)
        {
            _p -= decayPerSecond * Time.deltaTime;
            if (_p < 0f) _p = 0f;
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (fillRect != null)
        {
            var s = fillRect.sizeDelta;
            s.x = _fullWidth * _p;
            fillRect.sizeDelta = s;
        }

        if (statusText != null)
            statusText.text = Mathf.RoundToInt(_p * 100f) + "%";
    }

    private void EndWin()
    {
        _running = false;
        if (gamePanel != null) gamePanel.SetActive(false);
        _onWin?.Invoke();
    }
}
