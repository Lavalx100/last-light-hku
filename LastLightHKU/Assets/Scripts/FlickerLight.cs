using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light targetLight;
    public float baseIntensity = 1f;
    public float flickerAmount = 0.5f;
    public float flickerSpeed = 15f;

    void Reset()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (!targetLight) return;
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        targetLight.intensity = baseIntensity + (noise - 0.5f) * 2f * flickerAmount;
    }
}
