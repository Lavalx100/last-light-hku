using TMPro;
using UnityEngine;

public class TasksPageUI : MonoBehaviour
{
    [Header("Status Colors")]
    public Color completeColor = Color.green;
    public Color incompleteColor = Color.red;

    [Header("Map TaskIds to your four arcade games")]
    public TaskId signalTask = TaskId.TaskA;
    public TaskId pongTask = TaskId.TaskB;
    public TaskId frequencyTask = TaskId.TaskC;
    public TaskId lightTask = TaskId.TaskD;

    [Header("Text References")]
    public TMP_Text statusSignal;
    public TMP_Text statusPong;
    public TMP_Text statusFrequency;
    public TMP_Text statusLight;
    public TMP_Text exitStatus;
    public TMP_Text progressText; // optional

    void OnEnable()
    {
        HookEvents(true);
        RefreshAll();
    }

    void OnDisable()
    {
        HookEvents(false);
    }

    void HookEvents(bool on)
    {
        if (TaskManager.I == null) return;

        if (on)
        {
            TaskManager.I.OnTaskCompleted += HandleTaskCompleted;
            TaskManager.I.OnAllTasksCompleted += HandleAllTasksCompleted;
        }
        else
        {
            TaskManager.I.OnTaskCompleted -= HandleTaskCompleted;
            TaskManager.I.OnAllTasksCompleted -= HandleAllTasksCompleted;
        }
    }

    void HandleTaskCompleted(TaskId _)
    {
        RefreshAll();
    }

    void HandleAllTasksCompleted()
    {
        RefreshAll();
    }

    void RefreshAll()
    {
        if (TaskManager.I == null) return;

        SetStatus(statusSignal, TaskManager.I.IsComplete(signalTask));
        SetStatus(statusPong, TaskManager.I.IsComplete(pongTask));
        SetStatus(statusFrequency, TaskManager.I.IsComplete(frequencyTask));
        SetStatus(statusLight, TaskManager.I.IsComplete(lightTask));

        bool unlocked = TaskManager.I.AreAllTasksComplete();
        if (exitStatus != null)
        {
            exitStatus.text = unlocked ? "Exit Door: Unlocked" : "Exit Door: Locked";
            exitStatus.color = unlocked ? completeColor : incompleteColor;
        }

        if (progressText != null)
            progressText.text = $"{TaskManager.I.CompletedCount}/{TaskManager.I.RequiredCount} completed";
    }

    void SetStatus(TMP_Text t, bool complete)
    {
        if (t == null) return;
        t.text = complete ? "Complete" : "Incomplete";
        t.color = complete ? completeColor : incompleteColor;
    }
}
