using UnityEngine;

public class AcessOthersinventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private InventoryManager inventoryManager;

    public InventoryData otherInventoryData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        InventoryData inventoryData =
            other.GetComponent<InventoryData>();

        if (inventoryData != null)
        {
            otherInventoryData = inventoryData;

            Debug.Log(
                "Inventory detected: " +
                inventoryData.CharacterName
            );
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InventoryData inventoryData =
            other.GetComponent<InventoryData>();

        if (inventoryData == otherInventoryData)
        {
            otherInventoryData = null;

            Debug.Log("Inventory no longer detected.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenOtherInventory();
        }
    }

    private void OpenOtherInventory()
    {
        if (otherInventoryData == null)
        {
            Debug.Log("No character nearby.");
            return;
        }

        inventoryManager.UpdateInventoryUI(
            otherInventoryData
        );

        UI.instance.ToggleInventory();
    }
}