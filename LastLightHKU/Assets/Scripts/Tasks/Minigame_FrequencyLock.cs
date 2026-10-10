using UnityEngine;
using UnityEngine.UI;

public class Minigame_FrequencyLock : ConsoleGameBase
{
    [Header("UI")]
    public RectTransform track;      // TrackBG
    public RectTransform targetZone; // TargetZone
    public RectTransform cursorBar;  // CursorBar
    public Text lockText;

    [Header("Tuning")]
    public float speed = 260f;
    public float maxSpeed = 800f;
    public int locksToWin = 3;
    public KeyCode lockKey = KeyCode.Space;

    private int locks;
    private float dir = 1f;

    protected override void OnBegin()
    {
        locks = 0;
        dir = 1f;
        UpdateText();

        // Start cursor at left edge
        var p = cursorBar.anchoredPosition;
        p.x = -track.rect.width * 0.5f;
        cursorBar.anchoredPosition = p;
    }

    private void Update()
    {
        if (!running) return;

        var c = cursorBar.anchoredPosition;
        c.x += dir * speed * Time.deltaTime;

        float halfW = track.rect.width * 0.5f;
        if (c.x <= -halfW) { c.x = -halfW; dir = 1f; }
        if (c.x >= halfW)  { c.x = halfW;  dir = -1f; }

        cursorBar.anchoredPosition = c;

        if (Input.GetKeyDown(lockKey))
        {
            if (IsCursorInsideTarget())
            {
                locks++;
                UpdateText();

                // Move target zone to a new random location (max 150 units from center)
                float trackHalfWidth = track.rect.width * 0.5f;
                float targetHalfWidth = targetZone.rect.width * 0.5f;
                float minX = Mathf.Max(-trackHalfWidth + targetHalfWidth, -150f);
                float maxX = Mathf.Min(trackHalfWidth - targetHalfWidth, 150f);
                float newX = Random.Range(minX, maxX);
                var targetPos = targetZone.anchoredPosition;
                targetPos.x = newX;
                targetZone.anchoredPosition = targetPos;

                // Make it a bit harder each success
                speed = Mathf.Min(maxSpeed, speed * 1.12f);

                if (locks >= locksToWin)
                {
                    Win();
                    return;
                }
            }
            else
            {
                // Optional penalty: reset
                locks = 0;
                speed = Mathf.Max(200f, speed * 0.85f);
                UpdateText();
            }
        }
    }

    private bool IsCursorInsideTarget()
    {
        float cursorX = cursorBar.anchoredPosition.x;

        float targetCenter = targetZone.anchoredPosition.x;
        float halfZone = targetZone.rect.width * 0.5f;

        return cursorX >= targetCenter - halfZone && cursorX <= targetCenter + halfZone;
    }

    private void UpdateText()
    {
        if (lockText != null) lockText.text = $"Locks: {locks}/{locksToWin}";
    }
}
