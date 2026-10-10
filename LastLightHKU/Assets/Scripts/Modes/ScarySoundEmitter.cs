using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ScarySoundEmitter : MonoBehaviour
{
    [Header("Only plays in Scary mode")]
    public GameMode onlyInMode = GameMode.Scary;

    [Header("Clips")]
    public AudioClip[] clips;

    [Header("Playback")]
    public bool playOnStart = true;
    public bool loop = false;
    [Range(0f, 1f)] public float volume = 0.8f;

    [Tooltip("Random delay before first play")]
    public Vector2 startDelayRange = new Vector2(0f, 5f);

    [Tooltip("If not looping, wait random time then play again")]
    public bool randomRepeat = true;
    public Vector2 repeatDelayRange = new Vector2(10f, 30f);

    AudioSource src;
    float nextTime;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.spatialBlend = 1f; // 3D sound
        src.loop = loop;
        src.playOnAwake = false;
        src.volume = volume;
    }

    void Start()
    {
        ApplyMode();

        if (!enabled) return;

        if (playOnStart)
        {
            nextTime = Time.time + Random.Range(startDelayRange.x, startDelayRange.y);
        }
    }

    void Update()
    {
        ApplyMode();

        if (!enabled) return;

        if (loop)
        {
            if (!src.isPlaying) PlayRandom();
            return;
        }

        if (playOnStart && Time.time >= nextTime && !src.isPlaying)
        {
            PlayRandom();

            if (randomRepeat)
                nextTime = Time.time + Random.Range(repeatDelayRange.x, repeatDelayRange.y);
            else
                playOnStart = false;
        }
    }

    void ApplyMode()
    {
        var gm = GameModeManager.I;
        if (gm == null)
        {
            Debug.LogWarning("[ScarySoundEmitter] No GameModeManager in scene.");
            return;
        }

        bool ok = gm.CurrentMode == onlyInMode;
        enabled = ok;

        if (!ok && src != null) src.Stop();
    }

    void PlayRandom()
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        src.clip = clip;
        src.volume = volume;
        src.Play();

        Debug.Log($"[ScarySoundEmitter] Playing {clip.name} at {gameObject.name}");
    }
}