using UnityEngine;

/// <summary>
/// Persists the selected game mode from the main menu into gameplay
/// </summary>
public static class GameModeStore
{
    private const string Key = "SelectedGameMode";

    public static GameMode SelectedMode { get; private set; } = GameMode.Scary;

    /// <summary>
    /// Set and persist the game mode (called from main menu)
    /// </summary>
    public static void SetMode(GameMode mode)
    {
        SelectedMode = mode;
        PlayerPrefs.SetInt(Key, (int)mode);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load the persisted game mode (called when gameplay scene loads)
    /// </summary>
    public static GameMode LoadMode()
    {
        SelectedMode = (GameMode)PlayerPrefs.GetInt(Key, (int)GameMode.Scary);
        return SelectedMode;
    }
}
