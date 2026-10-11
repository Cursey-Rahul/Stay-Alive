
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

    public bool CanDriveAway =>
    completedTasks.Contains(returnToVanTaskName) &&
    activeTasks.ContainsKey(escapeTaskName);

    public int RequiredSurvivors =>
    FindMission("Find useful survivors")?.requiredAmount ?? 1;

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

         

            if (string.IsNullOrWhiteSpace(mission.taskName))
            {
                Debug.LogWarning("A mission has an empty Task Name!");
                continue;
            }

            if (mission.showAtStart)
                ShowTask(mission.taskName);
        }
    }
    public bool HasEnoughSurvivors(int survivorCount)
    {
        return survivorCount >= RequiredSurvivors;
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

    }


    public void AddProgress(string taskName, int amount = 1)
    {
        

        if (!activeTasks.TryGetValue(taskName, out MissionTaskUI task))
        {
            Debug.LogWarning(
                $"[MISSION] No active task found for '{taskName}'");
            return;
        }

       

        bool completed = task.AddProgress(amount);

      

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

    
    public void ReachVan()
    {
        if (!completedTasks.Contains(gearsTaskName) ||
            !completedTasks.Contains(fuelTaskName) ||
            !completedTasks.Contains(batteryTaskName))
        {
            
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
public void CompleteEscape()
    {
        if (!completedTasks.Contains(returnToVanTaskName) ||
            !activeTasks.TryGetValue(escapeTaskName, out MissionTaskUI task))
        {
            Debug.LogWarning("Escape objective is not ready.");
            return;
        }

        // Remove the final mission from the UI.
        task.Complete();
        activeTasks.Remove(escapeTaskName);
        completedTasks.Add(escapeTaskName);

        if (UI.instance != null)
            UI.instance.GameClear();
    }
}
