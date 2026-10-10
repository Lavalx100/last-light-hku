using System;
using UnityEngine;

public abstract class ConsoleGameBase : MonoBehaviour, IMinigame
{
    [Header("Required")]
    public GameObject gamePanel; // TaskPoint already shows/hides it, but we keep it for safety

    protected Action onWin;
    protected bool running;

    public virtual void Begin(Action onWinCallback)
    {
        onWin = onWinCallback;
        running = true;
        if (gamePanel != null) gamePanel.SetActive(true);
        OnBegin();
    }

    protected abstract void OnBegin();

    protected void Win()
    {
        running = false;
        if (gamePanel != null) gamePanel.SetActive(false);
        onWin?.Invoke();
    }

    protected void StopGame()
    {
        running = false;
        if (gamePanel != null) gamePanel.SetActive(false);
    }
}
