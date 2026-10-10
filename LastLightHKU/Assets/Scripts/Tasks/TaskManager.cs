using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager I { get; private set; }

    [Header("Required tasks to unlock exit")]
    [SerializeField] private List<TaskId> requiredTasks = new()
    {
        TaskId.TaskA, TaskId.TaskB, TaskId.TaskC, TaskId.TaskD
    };

    private readonly HashSet<TaskId> completed = new();

    public event Action<TaskId> OnTaskCompleted;
    public event Action OnAllTasksCompleted;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    public bool IsComplete(TaskId id) => completed.Contains(id);

    public void Complete(TaskId id)
    {
        if (!completed.Add(id)) return;

        OnTaskCompleted?.Invoke(id);

        if (AreAllTasksComplete())
            OnAllTasksCompleted?.Invoke();
    }

    public bool AreAllTasksComplete()
    {
        foreach (var t in requiredTasks)
            if (!completed.Contains(t)) return false;
        return true;
    }

    public int CompletedCount => completed.Count;
    public int RequiredCount => requiredTasks.Count;
}
