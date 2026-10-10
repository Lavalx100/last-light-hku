using System;
using UnityEngine;

public class Minigame_PressToComplete : MonoBehaviour, IMinigame
{
    public Canvas canvas; // simple overlay
    private Action onWin;

    private void Start()
    {
        if (canvas != null) canvas.enabled = false;
    }

    public void Begin(Action onWinCallback)
    {
        onWin = onWinCallback;

        if (canvas != null) canvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Time.timeScale != 0f) return;

        // Press Enter to complete the minigame
        if (Input.GetKeyDown(KeyCode.Return))
        {
            End();
            onWin?.Invoke();
        }

        // Press Escape to close without winning
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            End();
        }
    }

    private void End()
    {
        Time.timeScale = 1f;
        if (canvas != null) canvas.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
