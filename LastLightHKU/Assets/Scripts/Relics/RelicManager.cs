using System;
using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager I { get; private set; }

    [Header("Data")]
    public RelicCatalog catalog;

    public event Action<RelicId> OnRelicCollected;
    public event Action OnCollectionChanged;

    readonly HashSet<RelicId> collected = new();

    const string SaveKeyPrefix = "RELIC_";

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        LoadFromPrefs();
    }

    void LoadFromPrefs()
    {
        collected.Clear();

        if (catalog == null || catalog.relics == null) return;

        foreach (var def in catalog.relics)
        {
            if (def == null) continue;
            string key = SaveKeyPrefix + def.id;
            if (PlayerPrefs.GetInt(key, 0) == 1)
                collected.Add(def.id);
        }
    }

    void SaveOne(RelicId id, bool value)
    {
        string key = SaveKeyPrefix + id;
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsCollected(RelicId id) => collected.Contains(id);

    public int CollectedCount => collected.Count;

    public int TotalCount => (catalog != null) ? catalog.Count : 0;

    public bool TryCollect(RelicId id)
    {
        if (collected.Contains(id)) return false;

        collected.Add(id);
        SaveOne(id, true);

        Debug.Log($"[RelicManager] Collected relic: {id} ({CollectedCount}/{TotalCount})");

        OnRelicCollected?.Invoke(id);
        OnCollectionChanged?.Invoke();
        return true;
    }

    // Optional testing button
    [ContextMenu("RESET ALL RELICS")]
    public void ResetAll()
    {
        if (catalog == null || catalog.relics == null) return;

        collected.Clear();
        foreach (var def in catalog.relics)
        {
            if (def == null) continue;
            SaveOne(def.id, false);
        }

        Debug.Log("[RelicManager] Reset all relics.");
        OnCollectionChanged?.Invoke();
    }
}
