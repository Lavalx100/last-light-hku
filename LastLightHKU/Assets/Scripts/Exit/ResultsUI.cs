using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text finalTimeText;
    public TMP_Text bestTimeText;
    public TMP_Text foundText;

    [Header("Relics Grid")]
    public Transform gridParent;
    public RelicSlotUI slotPrefab;

    readonly List<RelicSlotUI> slots = new();

    void OnEnable()
    {
        BuildIfNeeded();

        float final = StopwatchManager.I != null ? StopwatchManager.I.ElapsedSeconds : 0f;

        RunRecords.SaveLast(final);
        bool isNewBest = RunRecords.TryUpdateBest(final);
        
        // Also save per-mode best time for main menu display
        if (GameModeManager.I != null)
            RunRecords.TryUpdateBestForMode(GameModeManager.I.CurrentMode, final);

        if (finalTimeText != null)
            finalTimeText.text = $"Time: {StopwatchManager.FormatTime(final)}";

        float best = RunRecords.LoadBest();
        if (bestTimeText != null)
        {
            if (best >= 0f)
                bestTimeText.text = isNewBest
                    ? $"Best: {StopwatchManager.FormatTime(best)}  (New!)"
                    : $"Best: {StopwatchManager.FormatTime(best)}";
            else
                bestTimeText.text = "Best: --:--.---";
        }

        RefreshRelics();
    }

    void BuildIfNeeded()
    {
        if (slots.Count > 0) return;
        if (RelicManager.I == null || RelicManager.I.catalog == null) return;
        if (gridParent == null || slotPrefab == null) return;

        var list = RelicManager.I.catalog.relics;
        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            var def = list[i];
            if (def == null) continue;

            var slot = Instantiate(slotPrefab, gridParent);
            slot.Bind(def);
            slots.Add(slot);
        }
    }

    void RefreshRelics()
    {
        if (RelicManager.I == null || RelicManager.I.catalog == null) return;

        int found = RelicManager.I.CollectedCount;
        int total = RelicManager.I.TotalCount;

        if (foundText != null)
            foundText.text = $"Found {found} / {total}";

        var list = RelicManager.I.catalog.relics;
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= list.Count) continue;
            var def = list[i];
            if (def == null) continue;

            bool collected = RelicManager.I.IsCollected(def.id);
            slots[i].SetCollected(collected);
        }
    }
}
