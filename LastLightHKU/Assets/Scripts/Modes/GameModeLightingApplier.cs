using UnityEngine;

public static class GameModeLightingApplier
{
    public static void Apply(GameModeConfig cfg)
    {
        if (cfg == null) return;

        // Skybox
        if (cfg.skybox != null)
            RenderSettings.skybox = cfg.skybox;

        // Fog
        RenderSettings.fog = cfg.enableFog;
        RenderSettings.fogColor = cfg.fogColor;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = cfg.fogDensity;

        // Ambient
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = cfg.ambientColor;
        RenderSettings.ambientIntensity = cfg.ambientIntensity;

        // Reflections
        RenderSettings.reflectionIntensity = cfg.reflectionIntensity;

        // Force refresh in editor/play
        DynamicGI.UpdateEnvironment();

        Debug.Log("[LightingApplier] Applied lighting settings.");
    }
}