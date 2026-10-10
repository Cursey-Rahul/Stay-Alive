
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [Header("Status Text")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI gearText;
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private TextMeshProUGUI killText;

    [Header("Inventory Data")]
    [SerializeField] private InventoryData inventoryData;

    private float elapsedTime;
    private int kills;

    private void Update()
    {
        UpdateTime();
        UpdateResources();
        UpdateKills();
    }

    private void UpdateTime()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (timeText != null)
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void UpdateResources()
    {
        if (inventoryData == null)
            return;

        gearText.text = $"Gear: {GetQuantity("Gear")}";
        fuelText.text = $"Fuel: {GetQuantity("Fuel")}";
        batteryText.text = $"Battery: {GetQuantity("Battery")}";
    }

    private int GetQuantity(string itemName)
    {
        Item[] items = inventoryData.Items;

        if (items == null)
            return 0;

        foreach (Item item in items)
        {
            if (item != null &&
                item.itemName.Equals(
                    itemName,
                    System.StringComparison.OrdinalIgnoreCase))
            {
                return item.quantity;
            }
        }

        return 0;
    }

    private void UpdateKills()
    {
        if (killText != null)
            killText.text = $"Kills: {kills}";
    }

    public void AddKill()
    {
        kills++;
    }

    public void ResetKills()
    {
        kills = 0;
    }
}
