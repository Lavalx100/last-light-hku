using UnityEngine;

public static class RunRecords
{
    const string BestKey = "BEST_TIME_SECONDS";
    const string LastKey = "LAST_TIME_SECONDS";
    
    // Per-mode keys for main menu display
    const string BestScaryKey = "BestTimeSeconds_Scary";
    const string BestEasyKey = "BestTimeSeconds_Easy";

    public static void SaveLast(float seconds)
    {
        PlayerPrefs.SetFloat(LastKey, seconds);
        PlayerPrefs.Save();
    }

    public static float LoadLast() => PlayerPrefs.GetFloat(LastKey, -1f);

    public static bool HasBest() => PlayerPrefs.HasKey(BestKey);

    public static float LoadBest() => PlayerPrefs.GetFloat(BestKey, -1f);

    public static bool TryUpdateBest(float seconds)
    {
        float best = LoadBest();
        if (best < 0f || seconds < best)
        {
            PlayerPrefs.SetFloat(BestKey, seconds);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // Per-mode best times for main menu
    public static bool TryUpdateBestForMode(GameMode mode, float seconds)
    {
        string key = mode == GameMode.Scary ? BestScaryKey : BestEasyKey;
        float best = PlayerPrefs.GetFloat(key, -1f);
        
        if (best < 0f || seconds < best)
        {
            PlayerPrefs.SetFloat(key, seconds);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
