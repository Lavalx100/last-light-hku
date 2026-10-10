using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.15f;
    public float gravity = -9.81f;

    [Header("Footstep Sound")]
    public AudioClip footstepSound;
    [Range(0f, 1f)]
    public float footstepVolume = 0.15f;
    [Range(0.8f, 1.2f)]
    public float pitchVariation = 0.1f; // Adds realism by varying pitch slightly

    float yVelocity;
    float xRotation;
    bool cameraMovementLocked = false;

    CharacterController controller;
    Transform cam;
    AudioSource audioSource;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>().transform;
        audioSource = GetComponent<AudioSource>();

        // Configure AudioSource for spatial footsteps
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // Full 3D spatial audio
        audioSource.volume = footstepVolume;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse look (only if camera movement is not locked and game is not paused)
        if (!cameraMovementLocked && !PauseMenuController.IsPaused)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;
            xRotation -= mouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseDelta.x);
        }

        // WASD movement
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;

        Vector3 move = (transform.right * input.x + transform.forward * input.y).normalized;

        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveSpeed * move + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Handle footstep sounds - play only when moving, grounded, and not paused
        bool shouldPlayFootsteps = input.magnitude > 0f && controller.isGrounded && !PauseMenuController.IsPaused;
        
        if (shouldPlayFootsteps)
        {
            if (!audioSource.isPlaying && footstepSound != null)
            {
                audioSource.clip = footstepSound;
                audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
                audioSource.Play();
            }
        }
        else
        {
            // Stop immediately when not moving or paused
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void SetCameraMovementLocked(bool locked)
    {
        cameraMovementLocked = locked;
    }
}
