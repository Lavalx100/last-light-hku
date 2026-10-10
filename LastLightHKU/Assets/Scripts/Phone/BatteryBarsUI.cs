using UnityEngine;

public class BatteryBarsUI : MonoBehaviour
{
    public Battery phoneBattery; // single source for both bars

    public RectTransform phoneFill;
    public RectTransform flashFill;

    float phoneMaxWidth;
    float flashMaxWidth;

    void Start()
    {
        if (phoneFill != null) phoneMaxWidth = phoneFill.sizeDelta.x;
        if (flashFill != null) flashMaxWidth = flashFill.sizeDelta.x;

        if (phoneBattery != null)
            phoneBattery.OnChargeChanged += HandleChargeChanged;

        UpdateBars();
    }

    void OnDestroy()
    {
        if (phoneBattery != null)
            phoneBattery.OnChargeChanged -= HandleChargeChanged;
    }

    void HandleChargeChanged(float current, float max)
    {
        UpdateBars();
    }

    void UpdateBars()
    {
        if (phoneBattery == null) return;

        float t = phoneBattery.Normalized();

        if (phoneFill != null)
            phoneFill.sizeDelta = new Vector2(phoneMaxWidth * t, phoneFill.sizeDelta.y);

        if (flashFill != null)
            flashFill.sizeDelta = new Vector2(flashMaxWidth * t, flashFill.sizeDelta.y);
    }
}
