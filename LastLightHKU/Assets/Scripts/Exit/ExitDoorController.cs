using UnityEngine;

public class ExitDoorController : MonoBehaviour
{
    [Header("Door structure")]
    public GameObject exitDoorRoot;     // your empty ExitDoor parent (contains DoorLeft and DoorRight)
    public GameObject doorLeft;         // optional reference
    public GameObject doorRight;        // optional reference

    [Header("UI (optional)")]
    public GameObject lockedMessageUI;  // "Complete all tasks" message
    public GameObject unlockedMessageUI; // "Exit unlocked" message (optional)
    public GameObject winUI;            // win overlay (optional)

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside;
    private bool hasWon;

    private void Start()
    {
        if (lockedMessageUI != null) lockedMessageUI.SetActive(false);
        if (unlockedMessageUI != null) unlockedMessageUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);

        // If tasks already complete (debug/testing), show unlocked state
        if (TaskManager.I != null && TaskManager.I.AreAllTasksComplete())
            SetUnlockedVisuals(true);
    }

    private void OnEnable()
    {
        if (TaskManager.I != null)
            TaskManager.I.OnAllTasksCompleted += HandleAllTasksCompleted;
    }

    private void OnDisable()
    {
        if (TaskManager.I != null)
            TaskManager.I.OnAllTasksCompleted -= HandleAllTasksCompleted;
    }

    private void Update()
    {
        if (!playerInside || hasWon) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (TaskManager.I != null && TaskManager.I.AreAllTasksComplete())
            {
                OpenExitByDisablingDoorRoot();
            }
            else
            {
                if (lockedMessageUI != null) lockedMessageUI.SetActive(true);
            }
        }
    }

    private void HandleAllTasksCompleted()
    {
        SetUnlockedVisuals(true);

        // If the player is standing there, clear the locked message instantly
        if (lockedMessageUI != null) lockedMessageUI.SetActive(false);
    }

    private void SetUnlockedVisuals(bool unlocked)
    {
        if (unlockedMessageUI != null) unlockedMessageUI.SetActive(unlocked && playerInside);
    }

    private void OpenExitByDisablingDoorRoot()
    {
        hasWon = true;

        // Optional: show win UI before disabling the door root
        if (winUI != null) winUI.SetActive(true);

        // This is what you asked for: disable the whole ExitDoor parent object
        if (exitDoorRoot != null) exitDoorRoot.SetActive(false);

        Debug.Log("WIN");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;

        if (lockedMessageUI != null) lockedMessageUI.SetActive(false);

        bool unlocked = (TaskManager.I != null && TaskManager.I.AreAllTasksComplete());
        if (unlockedMessageUI != null) unlockedMessageUI.SetActive(unlocked);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;

        if (lockedMessageUI != null) lockedMessageUI.SetActive(false);
        if (unlockedMessageUI != null) unlockedMessageUI.SetActive(false);
    }
}
