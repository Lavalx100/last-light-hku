using UnityEngine;

[CreateAssetMenu(menuName = "Game/Mode Config", fileName = "ModeConfig_")]
public class GameModeConfig : ScriptableObject
{
    public GameMode mode;

    [Header("Lighting")]
    public Material skybox;
    public bool enableFog = true;
    public Color fogColor = Color.black;
    [Range(0f, 1f)] public float fogDensity = 0.02f;

    [Header("Ambient / Reflections")]
    [Range(0f, 2f)] public float ambientIntensity = 1f;
    public Color ambientColor = Color.gray;
    [Range(0f, 2f)] public float reflectionIntensity = 1f;

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
}