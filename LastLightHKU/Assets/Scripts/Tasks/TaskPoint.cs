using UnityEngine;

public class TaskPoint : MonoBehaviour
{
    [Header("Task")]
    public TaskId taskId;

    [Header("References")]
    public Light interestLight;
    public MonoBehaviour miniGameBehaviour; // must implement IMinigame

    [Header("Console Screen Panels")]
    public GameObject promptPanel;
    public GameObject gamePanel;
    public GameObject donePanel;

    [Header("Completion Visuals (optional)")]
    public Behaviour flickerToDisable;

    private IMinigame miniGame;
    private bool playerInside;
    private bool running;
    private float baseLightIntensity;

    private void Awake()
    {
        miniGame = miniGameBehaviour as IMinigame;
        if (miniGame == null)
            Debug.LogError($"{name}: miniGameBehaviour must implement IMinigame.");

        if (interestLight != null) baseLightIntensity = interestLight.intensity;
    }

    private void OnEnable()
    {
        if (TaskManager.I != null)
            TaskManager.I.OnTaskCompleted += OnAnyTaskCompleted;
    }

    private void OnDisable()
    {
        if (TaskManager.I != null)
            TaskManager.I.OnTaskCompleted -= OnAnyTaskCompleted;
    }

    private void Start()
    {
        ApplyScreenState();
    }

    private void Update()
    {
        if (!playerInside) return;
        if (TaskManager.I == null) return;
        if (TaskManager.I.IsComplete(taskId)) return;
        if (running) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            running = true;
            ApplyScreenState(); // immediately hides prompt and shows game panel

            miniGame.Begin(() =>
            {
                TaskManager.I.Complete(taskId);
                running = false;
                ApplyScreenState(); // immediately shows DonePanel, no need to exit trigger
            });
        }
    }

    private void OnAnyTaskCompleted(TaskId id)
    {
        if (id != taskId) return;
        ApplyScreenState();
    }

    private void ApplyScreenState()
    {
        bool done = TaskManager.I != null && TaskManager.I.IsComplete(taskId);

        if (promptPanel != null) promptPanel.SetActive(playerInside && !done && !running);
        if (gamePanel != null) gamePanel.SetActive(playerInside && !done && running);
        if (donePanel != null) donePanel.SetActive(playerInside && done);

        if (done)
        {
            if (flickerToDisable != null) flickerToDisable.enabled = false;

            if (interestLight != null)
                interestLight.intensity = baseLightIntensity * 1.3f; // fixed, no repeated multiplying
        }
        else
        {
            if (interestLight != null)
                interestLight.intensity = baseLightIntensity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        ApplyScreenState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        running = false;

        if (promptPanel != null) promptPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (donePanel != null) donePanel.SetActive(false);
    }
}
