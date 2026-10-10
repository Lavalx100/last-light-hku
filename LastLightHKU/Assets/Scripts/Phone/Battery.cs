using System;
using UnityEngine;
//first property is phone battery
public class Battery : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxCharge = 100f;
    public float currentCharge = 100f;

    [Tooltip("Default drain rate per second when consuming.")]
    public float drainPerSecond = 5f;

    public bool IsEmpty => currentCharge <= 0.0001f;

    // UI update hook
    public event Action<float, float> OnChargeChanged; // (current, max)
    public event Action OnBecameEmpty;

    bool wasEmpty;

    void Awake()
    {
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        wasEmpty = IsEmpty;
        OnChargeChanged?.Invoke(currentCharge, maxCharge);
    }

    public void Drain(float amount)
    {
        if (amount <= 0f || IsEmpty) return;

        currentCharge = Mathf.Clamp(currentCharge - amount, 0f, maxCharge);
        OnChargeChanged?.Invoke(currentCharge, maxCharge);

        if (!wasEmpty && IsEmpty)
        {
            wasEmpty = true;
            OnBecameEmpty?.Invoke();
        }
    }

    public void AddCharge(float amount)
    {
        if (amount <= 0f) return;

        currentCharge = Mathf.Clamp(currentCharge + amount, 0f, maxCharge);
        OnChargeChanged?.Invoke(currentCharge, maxCharge);

        if (wasEmpty && !IsEmpty)
            wasEmpty = false;
    }

    public float Normalized() => maxCharge <= 0f ? 0f : currentCharge / maxCharge;
}
