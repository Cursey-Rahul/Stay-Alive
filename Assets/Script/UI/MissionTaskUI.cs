
using TMPro;
using UnityEngine;

public class MissionTaskUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;

    public string TaskName { get; private set; }

    private int currentAmount;
    private int requiredAmount;
    private bool permanent;

    public void Setup(string taskName, int required, bool isPermanent = false)
    {
        TaskName = taskName;
        requiredAmount = Mathf.Max(1, required);
        currentAmount = 0;
        permanent = isPermanent;

        UpdateText();
    }

    public bool AddProgress(int amount)
    {
        if (permanent)
            return false;

        currentAmount = Mathf.Clamp(
            currentAmount + amount,
            0,
            requiredAmount
        );
        


        UpdateText();

        if (currentAmount >= requiredAmount)
        {
            Complete();
            return true;
        }

        return false;
    }

    private void UpdateText()
    {
        if (taskText == null)
            return;

        if (permanent)
            taskText.text = TaskName;
        else
            taskText.text = $"{TaskName} ({currentAmount}/{requiredAmount})";
    }

    public void Complete()
    {
        Destroy(gameObject);
    }
}
