using UnityEngine;
using UnityEngine.UI;

public class Minigame_PongCalibration : ConsoleGameBase
{
    [Header("UI")]
    public RectTransform field;   // PongFieldBG rect
    public RectTransform paddle;  // Paddle rect
    public RectTransform ball;    // Ball rect
    public Text scoreText;

    [Header("Tuning")]
    public float paddleSpeed = 260f;
    public float ballSpeed = 220f;
    public int hitsToWin = 5;

    private Vector2 ballVel;
    private int hits;

    protected override void OnBegin()
    {
        hits = 0;
        UpdateScore();

        CenterBall();
        ballVel = new Vector2(ballSpeed, ballSpeed * 0.6f).normalized * ballSpeed;
        CenterPaddle();
    }

    private void Update()
    {
        if (!running) return;

        // Paddle movement
        float input = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input -= 1f;

        var p = paddle.anchoredPosition;
        p.y += input * paddleSpeed * Time.deltaTime;

        float halfH = field.rect.height * 0.5f;
        float padHalf = paddle.rect.height * 0.5f;
        p.y = Mathf.Clamp(p.y, -halfH + padHalf, halfH - padHalf);
        paddle.anchoredPosition = p;

        // Ball movement
        var b = ball.anchoredPosition;
        b += ballVel * Time.deltaTime;

        float halfW = field.rect.width * 0.5f;
        float ballHalf = ball.rect.width * 0.5f;

        // Top/bottom bounce
        if (b.y + ballHalf >= halfH || b.y - ballHalf <= -halfH)
        {
            ballVel.y *= -1f;
            b.y = Mathf.Clamp(b.y, -halfH + ballHalf, halfH - ballHalf);
        }

        // Right wall bounce
        if (b.x + ballHalf >= halfW)
        {
            ballVel.x *= -1f;
            b.x = halfW - ballHalf;
        }

        // Paddle collision (simple AABB)
        if (BallHitsPaddle(b))
        {
            ballVel.x = Mathf.Abs(ballVel.x); // ensure it goes right
            hits++;
            UpdateScore();

            // Slight difficulty ramp
            ballVel *= 1.05f;

            if (hits >= hitsToWin)
            {
                Win();
                return;
            }
        }

        // Miss on left side
        if (b.x - ballHalf <= -halfW)
        {
            // Reset
            hits = 0;
            UpdateScore();
            CenterBall();
            ballVel = new Vector2(ballSpeed, ballSpeed * 0.6f).normalized * ballSpeed;
        }

        ball.anchoredPosition = b;
    }

    private bool BallHitsPaddle(Vector2 ballPos)
    {
        var padPos = paddle.anchoredPosition;
        float padHalfW = paddle.rect.width * 0.5f;
        float padHalfH = paddle.rect.height * 0.5f;

        float ballHalf = ball.rect.width * 0.5f;

        bool withinX = ballPos.x - ballHalf <= padPos.x + padHalfW &&
                       ballPos.x + ballHalf >= padPos.x - padHalfW;

        bool withinY = ballPos.y <= padPos.y + padHalfH &&
                       ballPos.y >= padPos.y - padHalfH;

        // Only count if ball is moving left-to-right? Actually paddle is left side, ball moves left when coming back.
        // So collide when ball is heading left:
        bool headingLeft = ballVel.x < 0f;

        if (withinX && withinY && headingLeft)
        {
            // Add “spin” based on where it hit the paddle
            float offset = (ballPos.y - padPos.y) / padHalfH;
            ballVel = new Vector2(-ballVel.x, ballVel.y + offset * 120f).normalized * ballVel.magnitude;
            return true;
        }

        return false;
    }

    private void CenterBall()
    {
        ball.anchoredPosition = Vector2.zero;
    }

    private void CenterPaddle()
    {
        var p = paddle.anchoredPosition;
        p.y = 0f;
        paddle.anchoredPosition = p;
    }

    private void UpdateScore()
    {
        if (scoreText != null) scoreText.text = $"Hits: {hits}/{hitsToWin}";
    }
}
