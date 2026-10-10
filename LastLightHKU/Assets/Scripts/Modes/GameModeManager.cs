using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager I { get; private set; }

    [Header("Select mode in Inspector (for now)")]
    public GameMode selectedMode = GameMode.Scary;

    [Header("Configs")]
    public GameModeConfig scaryConfig;
    public GameModeConfig easyConfig;

    public GameMode CurrentMode { get; private set; }
    public GameModeConfig CurrentConfig { get; private set; }

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    void Start()
    {
        // Load mode from main menu selection (if coming from menu)
        selectedMode = GameModeStore.LoadMode();
        
        ApplySelectedMode();
    }

    public void ApplySelectedMode()
    {
        CurrentMode = selectedMode;

        CurrentConfig = selectedMode == GameMode.Scary ? scaryConfig : easyConfig;

        if (CurrentConfig == null)
        {
            Debug.LogError("[GameModeManager] Missing config for selected mode.");
            return;
        }

        Debug.Log($"[GameModeManager] Mode = {CurrentMode}");
        GameModeLightingApplier.Apply(CurrentConfig);
        MusicPlayer.I?.Apply(CurrentConfig);
    }
}