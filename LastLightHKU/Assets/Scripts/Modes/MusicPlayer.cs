using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer I { get; private set; }

    AudioSource src;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D music
    }

    public void Apply(GameModeConfig cfg)
    {
        if (cfg == null) return;

        src.volume = cfg.musicVolume;

        if (cfg.backgroundMusic == null)
        {
            src.Stop();
            Debug.LogWarning("[MusicPlayer] No background music assigned.");
            return;
        }

        if (src.clip == cfg.backgroundMusic && src.isPlaying) return;

        src.clip = cfg.backgroundMusic;
        src.Play();

        Debug.Log("[MusicPlayer] Music applied: " + cfg.backgroundMusic.name);
    }
}