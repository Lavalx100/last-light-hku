using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RelicsPageUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text foundText;
    public Transform gridParent;
    public RelicSlotUI slotPrefab;

    readonly List<RelicSlotUI> slots = new();

    void OnEnable()
    {
        BuildIfNeeded();
        HookEvents(true);
        Refresh();
    }

    void OnDisable()
    {
        HookEvents(false);
    }

    void HookEvents(bool on)
    {
        if (RelicManager.I == null) return;

        if (on)
            RelicManager.I.OnCollectionChanged += Refresh;
        else
            RelicManager.I.OnCollectionChanged -= Refresh;
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

    void Refresh()
    {
        if (RelicManager.I == null || RelicManager.I.catalog == null) return;

        int found = RelicManager.I.CollectedCount;
        int total = RelicManager.I.TotalCount;

        if (foundText != null)
            foundText.text = $"Found {found} / {total}";

        // Update each slot
        var list = RelicManager.I.catalog.relics;
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= list.Count) continue;
            var def = list[i];
            if (def == null) continue;

            bool isCollected = RelicManager.I.IsCollected(def.id);
            slots[i].SetCollected(isCollected);
        }
    }
}
