using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PhoneController : MonoBehaviour
{
    [Header("References")]
    public GameObject phoneUIRoot;  // drag PhoneRoot here
    public Battery phoneBattery;    // single battery for both phone + flashlight
    public Light phoneFlashlight;   // Spot Light attached to camera
    public FPSController fpsController;  // drag FPSController here

    [Header("Toggle Keys")]
    public Key togglePhoneKey = Key.Tab;
    public Key toggleFlashlightKey = Key.F;

    [Header("Drain")]
    public float drainWhenPhoneOpenPerSecond = 0.7f;
    public float drainWhenFlashlightOnPerSecond = 5.0f;

    [Header("Controls While Phone Open")]
    public bool unlockCursorWhenPhoneOpen = true;

    [Header("State (read-only during play)")]
    public bool phoneUIOpen;
    public bool flashlightOn;

    void Start()
    {
        if (phoneUIRoot != null)
            phoneUIRoot.SetActive(false);

        flashlightOn = false;
        ApplyFlashlight();
        ApplyCursorState();

        if (phoneBattery != null)
            phoneBattery.OnBecameEmpty += OnBatteryEmpty;
    }

    void OnDestroy()
    {
        if (phoneBattery != null)
            phoneBattery.OnBecameEmpty -= OnBatteryEmpty;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Toggle phone UI
        if (WasPressed(togglePhoneKey))
            TryTogglePhoneUI();

        // Toggle flashlight anytime (does not require phone UI open)
        if (WasPressed(toggleFlashlightKey))
        {
            if (phoneBattery != null && !phoneBattery.IsEmpty)
            {
                flashlightOn = !flashlightOn;
                ApplyFlashlight();
            }
        }

        if (phoneBattery == null) return;

        // If battery dead, force everything off
        if (phoneBattery.IsEmpty)
        {
            ForceOffEverything();
            return;
        }

        // Drain
        float drain = 0f;
        if (phoneUIOpen) drain += drainWhenPhoneOpenPerSecond;
        if (flashlightOn) drain += drainWhenFlashlightOnPerSecond;

        if (drain > 0f)
            phoneBattery.Drain(drain * Time.deltaTime);

        // Handle camera movement locking based on cursor position over phone UI
        HandleCameraLocking();
    }

    void TryTogglePhoneUI()
    {
        if (phoneBattery == null) return;
        if (phoneBattery.IsEmpty) return;

        phoneUIOpen = !phoneUIOpen;

        if (phoneUIRoot != null)
            phoneUIRoot.SetActive(phoneUIOpen);

        ApplyCursorState();
    }

    void ApplyFlashlight()
    {
        if (phoneFlashlight != null)
            phoneFlashlight.enabled = flashlightOn;
    }

    void ApplyCursorState()
    {
        if (!unlockCursorWhenPhoneOpen) return;

        if (phoneUIOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnBatteryEmpty()
    {
        ForceOffEverything();
    }

    void ForceOffEverything()
    {
        phoneUIOpen = false;
        flashlightOn = false;

        if (phoneUIRoot != null)
            phoneUIRoot.SetActive(false);

        ApplyFlashlight();
        ApplyCursorState();
    }

    void HandleCameraLocking()
    {
        if (fpsController == null) return;

        // Only lock camera if phone UI is open and cursor is over the phone UI
        bool shouldLockCamera = phoneUIOpen && IsCursorOverPhoneUI();
        fpsController.SetCameraMovementLocked(shouldLockCamera);
    }

    bool IsCursorOverPhoneUI()
    {
        if (phoneUIRoot == null || !phoneUIRoot.activeInHierarchy) return false;

        // Check if cursor is over any UI element (which would be the phone UI when it's open)
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    bool WasPressed(Key k)
    {
        var key = Keyboard.current[k];
        return key != null && key.wasPressedThisFrame;
    }
}
