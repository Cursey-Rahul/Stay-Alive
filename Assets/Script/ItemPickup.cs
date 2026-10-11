
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private string missionTaskName = "";

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        if (!CompareTag("Collectable"))
            return;

        Player player = other.GetComponentInParent<Player>();

        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return;
        }

        InventoryData inventory = player.InventoryData;

        if (inventory == null)
        {
            Debug.LogError("InventoryData is not assigned in Player.cs!");
            return;
        }

        string objectName = gameObject.name.ToLowerInvariant();
        string itemName;

        if (objectName.Contains("gear"))
            itemName = "Gear";
        else if (objectName.Contains("fuel"))
            itemName = "Fuel";
        else if (objectName.Contains("battery"))
            itemName = "Battery";
        else
            itemName = gameObject.name;

        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("Pickup sprite not found: " + name);
            return;
        }

        // Always add the collected item to the inventory.
        inventory.AddItem(itemName, spriteRenderer.sprite);

        collected = true;

       

        player.RefreshInventoryUI();

        // Update mission progress only if a mission name is assigned.
        if (!string.IsNullOrWhiteSpace(missionTaskName))
        {
            if (MissionManager.instance != null)
            {
                
                MissionManager.instance.AddProgress(missionTaskName);
            }
            else
            {
                Debug.LogWarning(
                    "No MissionManager found for: " + missionTaskName);
            }
        }

        Destroy(gameObject);
    }
}
