
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionDefinition
{
    public string taskName;
    public int requiredAmount = 1;

    [Tooltip("Enable for missions visible from the beginning.")]
    public bool showAtStart = true;

    [Tooltip("Enable for optional missions that never disappear.")]
    public bool permanent = false;
}

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    [Header("Mission UI")]
    [SerializeField] private Transform taskParent;
    [SerializeField] private MissionTaskUI taskPrefab;

    [Header("Tasks")]
    [SerializeField] private MissionDefinition[] missions;

    [Header("Resource Mission Names")]
    [SerializeField] private string gearsTaskName = "Find gears for the van";
    [SerializeField] private string fuelTaskName = "Find fuel for the van";
    [SerializeField] private string batteryTaskName = "Find battery for the van";

    [Header("Van Mission Names")]
    [SerializeField] private string returnToVanTaskName = "Fall back to the van";
    [SerializeField] private string escapeTaskName = "Drive away";

    private readonly Dictionary<string, MissionTaskUI> activeTasks =
        new Dictionary<string, MissionTaskUI>();

    private readonly HashSet<string> completedTasks =
        new HashSet<string>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (taskParent == null || taskPrefab == null)
        {
            Debug.LogError("Assign Task Parent and Task Prefab in MissionManager.");
            return;
        }

        foreach (MissionDefinition mission in missions)
        {
            if (mission == null)
                continue;

            Debug.Log("Mission name: [" + mission.taskName + "]");

            if (string.IsNullOrWhiteSpace(mission.taskName))
            {
                Debug.LogWarning("A mission has an empty Task Name!");
                continue;
            }

            if (mission.showAtStart)
                ShowTask(mission.taskName);
        }
    }

    private MissionDefinition FindMission(string taskName)
    {
        foreach (MissionDefinition mission in missions)
        {
            if (mission != null && mission.taskName == taskName)
                return mission;
        }

        Debug.LogError("Mission definition not found: " + taskName);
        return null;
    }

    private void ShowTask(string taskName)
    {
        if (activeTasks.ContainsKey(taskName) ||
            completedTasks.Contains(taskName))
            return;

        MissionDefinition definition = FindMission(taskName);

        if (definition == null)
            return;

        MissionTaskUI task = Instantiate(taskPrefab, taskParent);
        task.Setup(
            definition.taskName,
            definition.requiredAmount,
            definition.permanent
        );

        activeTasks.Add(taskName, task);

        Debug.Log("Activated mission: [" + taskName + "]");
    }


    public void AddProgress(string taskName, int amount = 1)
    {
        Debug.Log($"[MISSION] Received: '{taskName}'");

        if (!activeTasks.TryGetValue(taskName, out MissionTaskUI task))
        {
            Debug.LogWarning(
                $"[MISSION] No active task found for '{taskName}'");
            return;
        }

        Debug.Log(
            $"[MISSION] Updating UI object ID: {task.GetInstanceID()} " +
            $"for task: '{task.TaskName}'");

        bool completed = task.AddProgress(amount);

        Debug.Log(
            $"[MISSION] Progress sent to: '{task.TaskName}', " +
            $"Completed: {completed}");

        if (completed)
        {
            activeTasks.Remove(taskName);
            completedTasks.Add(taskName);

            CheckResourceObjectives();
        }
    }


    private void CheckResourceObjectives()
    {
        bool gearsDone = completedTasks.Contains(gearsTaskName);
        bool fuelDone = completedTasks.Contains(fuelTaskName);
        bool batteryDone = completedTasks.Contains(batteryTaskName);

        if (gearsDone && fuelDone && batteryDone)
            ShowTask(returnToVanTaskName);
    }

    // Call this when the player reaches the van
    // and the return-to-van objective is complete.
    public void ReachVan()
    {
        if (!completedTasks.Contains(gearsTaskName) ||
            !completedTasks.Contains(fuelTaskName) ||
            !completedTasks.Contains(batteryTaskName))
        {
            Debug.Log("Collect all required van resources first.");
            return;
        }

        if (!activeTasks.ContainsKey(returnToVanTaskName))
        {
            Debug.LogWarning("Return-to-van mission is not active.");
            return;
        }

        activeTasks[returnToVanTaskName].Complete();
        activeTasks.Remove(returnToVanTaskName);
        completedTasks.Add(returnToVanTaskName);

        ShowTask(escapeTaskName);
    }

    // Call this when the player successfully escapes.
    public void CompleteEscape()
    {
        if (!completedTasks.Contains(returnToVanTaskName) ||
            !activeTasks.ContainsKey(escapeTaskName))
        {
            Debug.LogWarning("Escape objective is not ready.");
            return;
        }

        activeTasks[escapeTaskName].Complete();
        activeTasks.Remove(escapeTaskName);
        completedTasks.Add(escapeTaskName);

        if (UI.instance != null)
            UI.instance.GameClear();
    }
}
