using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_LightSequence : ConsoleGameBase
{
    [Header("UI Tiles (Images)")]
    public Image tile1;
    public Image tile2;
    public Image tile3;
    public Image tile4;
    public Text roundText;

    [Header("Tuning")]
    public int sequenceLength = 4;
    public float flashOnTime = 0.25f;
    public float flashOffTime = 0.18f;

    private List<int> seq = new();
    private int inputIndex;
    private bool acceptingInput;

    private Color base1, base2, base3, base4;

    private void Awake()
    {
        if (tile1 != null) base1 = tile1.color;
        if (tile2 != null) base2 = tile2.color;
        if (tile3 != null) base3 = tile3.color;
        if (tile4 != null) base4 = tile4.color;
    }

    protected override void OnBegin()
    {
        seq.Clear();
        inputIndex = 0;
        acceptingInput = false;

        for (int i = 0; i < sequenceLength; i++)
            seq.Add(Random.Range(1, 5)); // 1..4

        if (roundText != null) roundText.text = $"Sequence: {sequenceLength}";
        StartCoroutine(ShowSequence());
    }

    private void Update()
    {
        if (!running) return;
        if (!acceptingInput) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) Register(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Register(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Register(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Register(4);
    }

    private void Register(int value)
    {
        Flash(value, 0.12f);

        if (seq[inputIndex] != value)
        {
            // Fail: replay sequence
            acceptingInput = false;
            inputIndex = 0;
            StartCoroutine(ShowSequence());
            return;
        }

        inputIndex++;
        if (inputIndex >= seq.Count)
        {
            Win();
        }
    }

    private IEnumerator ShowSequence()
    {
        acceptingInput = false;
        ResetTiles();

        yield return new WaitForSeconds(0.25f);

        foreach (var v in seq)
        {
            SetTile(v, true);
            yield return new WaitForSeconds(flashOnTime);

            SetTile(v, false);
            yield return new WaitForSeconds(flashOffTime);
        }

        inputIndex = 0;
        acceptingInput = true;
    }

    private void Flash(int v, float t)
    {
        StartCoroutine(FlashCo(v, t));
    }

    private IEnumerator FlashCo(int v, float t)
    {
        SetTile(v, true);
        yield return new WaitForSeconds(t);
        SetTile(v, false);
    }

    private void SetTile(int v, bool on)
    {
        var bright = Color.white;

        if (v == 1 && tile1 != null) tile1.color = on ? bright : base1;
        if (v == 2 && tile2 != null) tile2.color = on ? bright : base2;
        if (v == 3 && tile3 != null) tile3.color = on ? bright : base3;
        if (v == 4 && tile4 != null) tile4.color = on ? bright : base4;
    }

    private void ResetTiles()
    {
        if (tile1 != null) tile1.color = base1;
        if (tile2 != null) tile2.color = base2;
        if (tile3 != null) tile3.color = base3;
        if (tile4 != null) tile4.color = base4;
    }
}
